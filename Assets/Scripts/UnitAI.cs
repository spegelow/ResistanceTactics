using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAI : MonoBehaviour
{
    public Unit unit;

    [Header("AI Personality Parameters")]
    public float aggression;
    public float caution;



    // Start is called before the first frame update
    void Start()
    {
        if(unit == null)
        {
            unit = this.GetComponent<Unit>();
        }
    }

    [System.Obsolete("This is the old AI method, use ResolveTurn instead")]
    public IEnumerator BeginTurn()
    {
        yield return new WaitForSeconds(1);
        Debug.Log("Begin AI Turn");

        //Determine our ideal target
        Unit idealTarget = null;
        List<Unit> potentialTargets = new List<Unit>();
        //Are there any enemies in our attack range already?
        unit.GetAttackableTiles().ForEach(tile =>
        {
            if(tile.occupant != null && tile.occupant.team != unit.team)
            {
                potentialTargets.Add(tile.occupant);
            }
        });

        if(potentialTargets.Count > 0)
        {
            //There are targets in range, so pick the one with the lowest health and then that is the closest
            potentialTargets.Sort((a, b) => 
            {
                //Pick the one with the lowest health first
                int ret = a.currentHealth.CompareTo(b.currentHealth);
                if(ret == 0)
                {
                    //Current healths were equal, so pick the closest one
                    ret = unit.MovementRequiredToGetInRange(a).CompareTo(unit.MovementRequiredToGetInRange(b));
                }
                return ret;
            });
            idealTarget = potentialTargets[0];
        }

        if (idealTarget != null)
        {
            //Debug.Log(unit.unitName + " AI is targetting " + idealTarget?.unitName);
            //No movement is needed so don't move
            //Just attack that target
            yield return new WaitForSeconds(1);
            yield return StartCoroutine(BattleManager.instance.ResolveAttack(unit, idealTarget.currentTile));
        }
        else
        {
            //No target in range, find the closest enemy and move towards them
            potentialTargets = unit.GetAllEnemies();
            if(potentialTargets.Count == 0)
            {
                //There should always be at least one enemy...
                //I guess just end turn??
                Debug.LogError("There are no enemies left? Battle should be done...");
                BattleManager.instance.EndTurn();
                yield break;
            }
            potentialTargets.Sort((a, b) =>
            {
                //Pick the closest enemy
                int ret = unit.MovementRequiredToGetInRange(a).CompareTo(unit.MovementRequiredToGetInRange(b));
                return ret;
            });
            idealTarget = potentialTargets[0];
            //Debug.Log(unit.unitName + " AI is targetting " + idealTarget?.unitName);

            //Determine which tile to move to
            List<MapTile> moveableTiles = unit.GetMoveableTiles();
            //Find the tile closest to the ideal target
            moveableTiles.Sort((a, b) =>
            {
                int ret = unit.MovementRequiredToReachTile(a, idealTarget.currentTile).CompareTo(unit.MovementRequiredToReachTile(b, idealTarget.currentTile));
                return ret;
            });

            //Actually move to that tile
            yield return BattleManager.instance.MoveUnit(unit, moveableTiles[0], false);
            //Now check if our target is in range. If so, attack them. If not, end turn
            if (unit.GetAttackableTiles().Contains(idealTarget.currentTile))
            {
                yield return StartCoroutine(BattleManager.instance.ResolveAttack(unit, idealTarget.currentTile));
            }
            else
            {
                //Just end this AI's turn
                Debug.Log(unit.unitData.unitName + " is waiting.");
                yield return new WaitForSeconds(1);
                BattleManager.instance.EndTurn();
            }
        }
    }

    public IEnumerator ResolveTurn()
    {
        yield return new WaitForSeconds(1);
        Debug.Log("Begin AI Turn");

        //The AI's turn is a combination of moving, choosing an action, and choosing a target
        //For now, I guess we can brute force check all of those. 
        //TODO Reduce number of options to evaluate

        List<MapTile> moveableTiles = unit.GetMoveableTiles();
        float maxScore = float.MinValue;
        MapTile bestTile = unit.currentTile;
        float tileScore;
        foreach(MapTile tile in moveableTiles)
        {
            tileScore = 0;
            tileScore += GetTileDefensiveValue(tile) * caution;
            tileScore += GetTileOffensiveValue(tile) * aggression;

            if(tileScore > maxScore)
            {
                maxScore = tileScore;
                bestTile = tile;
            }
        }

        //We've calculated the best tile to move to, now determine the best action there (attack target for now)
        maxScore = float.MinValue;
        List<Unit> enemies = unit.GetAllEnemies();
        Unit bestTarget = null;
        foreach (Unit e in enemies)
        {
            tileScore = GetAttackValue(bestTile, e);
            if (tileScore > maxScore)
            {
                maxScore = tileScore;
                bestTarget = e;
            }
        }

        //Move to the best tile
        yield return BattleManager.instance.MoveUnit(unit, bestTile, false);

        //If we can attack the best target, do so
        if(unit.GetAttackableTiles().Contains(bestTarget.currentTile))
        {
            yield return StartCoroutine(BattleManager.instance.ResolveAttack(unit, bestTarget.currentTile));
        }
        else
        {
            //Just end this AI's turn
            Debug.Log(unit.unitData.unitName + " is waiting.");
            yield return new WaitForSeconds(1);
            BattleManager.instance.EndTurn();
        }
    }

    public float GetTileOffensiveValue(MapTile tile)
    {
        float finalValue = -1000;

        //For each enemy, check whether we can attack that enemy
        List<Unit> enemies = unit.GetAllEnemies();

        float enemyScore;
        foreach (Unit enemy in enemies)
        {
            enemyScore = GetAttackValue(tile, enemy);

            if(enemyScore > finalValue)
            {
                finalValue = enemyScore;
            }
        }

        return finalValue;
    }

    public float GetAttackValue(MapTile tile, Unit enemy)
    {
        float enemyScore = 0;

        //Can we attack the enemy from where we are now
        bool canSeeCurrentTile = unit.CanSeeTile(enemy.currentTile);
        int currentCover = enemy.GetCoverFromPosition(unit.currentTile);
        float accuracy = unit.CalculateAccuracy(unit.currentTile, enemy.currentTile);

        //Can we attack the enemy from the potential movement tile
        bool canSeeTile = unit.CheckLineOfSight(tile, enemy.currentTile);
        int newCover = enemy.GetCoverFromPosition(tile, enemy.currentTile);
        float newAccuracy = unit.CalculateAccuracy(tile, enemy.currentTile);

        //What is the range to the tile
        float range = tile.GetDistance(enemy.currentTile);

        //We only really care about tiles we can see and shoot at
        if (canSeeTile && range >= unit.unitData.weapon.minAttackRange && range <= unit.unitData.weapon.maxAttackRange)
        {
            //We can shoot this tile, so that is worth something
            enemyScore = 500;

            //Adjust value based on accuracy
            enemyScore *= newAccuracy / 100;

            //Bonuses:
            //High accuracy rewarded
            if (newAccuracy > 85)
            {
                enemyScore += 100;
            }

            //Prioritize potential kills
            if (enemy.currentHealth <= unit.unitData.weapon.maxDamage)
            {
                enemyScore += 50;
            }

            //Prioritize guarenteed kills
            if (enemy.currentHealth <= unit.unitData.weapon.minDamage)
            {
                enemyScore += 100;
            }

            //Penalties:
            //Outside of weapon's ideal range
            if (range > unit.unitData.weapon.idealRange)
            {
                enemyScore -= 50;
            }

            //Low accuracy penalized extra
            if (newAccuracy < 50)
            {
                enemyScore -= 50;
            }

            return enemyScore;
        }

        //Couldn't see enemy
        return -1000;
    }

    public float GetTileDefensiveValue(MapTile tile)
    {
        float finalValue = 0;

        //For each enemy, check whether that enemy could attack us and how cover is
        List<Unit> enemies = unit.GetAllEnemies();

        //TODO Check enemy range and weapons as well
        float enemyScore;
        foreach (Unit enemy in enemies)
        {
            enemyScore = 0;

            //Can this enemy see our current tile
            bool canSeeCurrentTile = enemy.CanSeeTile(unit.currentTile);
            int currentCover = unit.GetCoverFromPosition(enemy.currentTile);

            //Can this enemy see the potential movement tile
            bool canSeeTile = enemy.CanSeeTile(tile);
            int newCover = unit.GetCoverFromPosition(enemy.currentTile, tile);

            //So the ai should prioritize gaining/maintaining cover against enemies already in sight
            
            //Bonuses:
            if (canSeeCurrentTile && newCover > currentCover)
            {
                //Cover is better than before against an enemy we could see
                enemyScore += 50;
            }

            if (!canSeeTile && canSeeCurrentTile)
            {
                //Broke line of sight from previous enemy
                enemyScore += 25;
            }

            if (canSeeTile && newCover > 0)
            {
                //Has cover from visible enemy
                enemyScore += (newCover * 50);
            }

            if (canSeeCurrentTile && newCover > 0)
            {
                //Has cover from enemy that saw us last turn
                enemyScore += (newCover * 10);
            }

            //Penalties:
            if (canSeeCurrentTile && newCover < currentCover)
            {
                //Cover is worse than before against an enemy we could see
                enemyScore -= 25;
            }

            if (canSeeTile && !canSeeCurrentTile)
            {
                //New enemy has line of sight
                enemyScore -= 50;
            }

            if (canSeeTile && newCover == 0)
            {
                //Has no cover from enemy with LoS
                enemyScore -= 200;
            }

            if (newCover == 0)
            {
                //Has no cover from enemy with LoS
                enemyScore -= 10;
            }

            finalValue += enemyScore;
        }

        return finalValue;
    }
}
