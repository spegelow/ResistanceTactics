using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAI : MonoBehaviour
{
    public Unit unit;

    // Start is called before the first frame update
    void Start()
    {
        if(unit == null)
        {
            unit = this.GetComponent<Unit>();
        }
    }

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
            BattleManager.instance.AIMoveUnit(unit, moveableTiles[0]);
            yield return new WaitForSeconds(1);
            //Now check if our target is in range. If so, attack them. If not, end turn
            if (unit.GetAttackableTiles().Contains(idealTarget.currentTile))
            {
                yield return StartCoroutine(BattleManager.instance.ResolveAttack(unit, idealTarget.currentTile));
            }
            else
            {
                //Just end this AI's turn
                Debug.Log(unit.unitName + " is waiting.");
                yield return new WaitForSeconds(1);
                BattleManager.instance.EndTurn();
            }
        }
    }
}
