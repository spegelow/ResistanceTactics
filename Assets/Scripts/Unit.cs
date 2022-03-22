using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Unit : MonoBehaviour
{
    public UnitData unitData;

    public Vector2 mapPosition;

    public MapTile currentTile;
    public MapTile originalTile;//The tile this unit began movement on, used for undoing movement

    public int team;

    public int currentHealth;

    public List<Transform> targetingPoints;

    [Header("Inventory")]




    [Header("AI Settings")]
    public bool isAIControlled;
    public UnitAI unitAI;


    [Header("Events")]
    public UnityEvent OnHealthChanged;
    public UnityEvent OnUnitMoved;

    public void InitializeUnit(UnitData unitData, int team, Vector2 tilePosition)
    {
        this.unitData = unitData;
        this.team = team;
        mapPosition = tilePosition;

        if (currentTile == null)
        {
            MoveToTile(MapManager.instance.GetTile((int)mapPosition.x, (int)mapPosition.y));
        }
        currentTile.occupant = this;
        transform.position = currentTile.GetSurfacePosition();

        currentHealth = GetMaxHealth();
        OnHealthChanged.Invoke();
    }

    public void MoveToTile(MapTile newTile)
    {
        //if(newTile == currentTile)
        //{
        //    //Nothing needed
        //    return;
        //}

        if (newTile.IsOccupied() && newTile.occupant != this)
        {
            Debug.LogError("Cannot move to an occupied tile");
            return;
        }

        //Remove this unit from the previous tile
        if(currentTile!=null)
        {
            currentTile.occupant = null;
        }

        //Place the unit at the new tile
        originalTile = currentTile;
        currentTile = newTile;
        newTile.occupant = this;
        this.transform.position = newTile.GetSurfacePosition();

        OnUnitMoved.Invoke();
    }

    public void UndoMovement()
    {
        //Remove this unit from the tile
        currentTile.occupant = null;

        //Place the unit at its previous tile
        currentTile = originalTile;
        originalTile = null;
        currentTile.occupant = this;
        this.transform.position = currentTile.GetSurfacePosition();

        OnUnitMoved.Invoke();
    }

    public List<MapTile> GetMoveableTiles()
    {
        return Pathfinder.GetMoveableTiles(this);
        //return MapManager.instance.GetTilesInRange(currentTile.x, currentTile.z, movementRange);
    }

    public List<MapTile> GetAttackableTiles()
    {
        List<MapTile> targetableTiles = MapManager.instance.GetTilesInRange(currentTile.x, currentTile.z, unitData.weapon.minAttackRange, unitData.weapon.maxAttackRange);

        //Only return the tiles in line of sight
        return targetableTiles.FindAll(tile => CanSeeTile(tile));
    
    }

    public bool CanSeeTile(MapTile tile)
    {
        Vector3 attackOffset = new Vector3(0, 1.5f, 0);//Used to check line of sight from roughly the units face

        foreach(Transform point in this.targetingPoints)
        {
            foreach(Transform secondPoint in this.targetingPoints)
            {
                Vector3 offset = secondPoint.localPosition;
                if (!Physics.Linecast(point.position, tile.GetSurfacePosition() + offset))
                {
                    //Line of sight was drawn between corners
                    return true;
                }
            }
        }

        return false;
        //return !Physics.Linecast(this.currentTile.GetSurfacePosition() + attackOffset, tile.GetSurfacePosition() + attackOffset);
    }

    public bool CheckLineOfSight(MapTile tile1, MapTile tile2)
    {
        Vector3 attackOffset = new Vector3(0, 1.5f, 0);//Used to check line of sight from roughly the units face

        foreach (Transform point in this.targetingPoints)
        {
            Vector3 offset1 = point.localPosition;
            foreach (Transform secondPoint in this.targetingPoints)
            {
                Vector3 offset2 = secondPoint.localPosition;
                if (!Physics.Linecast(tile1.GetSurfacePosition() + offset1, tile2.GetSurfacePosition() + offset2))
                {
                    //Line of sight was drawn between corners
                    return true;
                }
            }
        }

        return false;
        //return !Physics.Linecast(this.currentTile.GetSurfacePosition() + attackOffset, tile.GetSurfacePosition() + attackOffset);
    }

    public bool IsValidTarget(MapTile tile)
    {
        return tile.occupant != null && tile.occupant.team != team;
    }

    /// <summary>
    /// Returns 0 for no cover, 1 for half-cover, 2 for full cover
    /// </summary>
    /// <returns></returns>
    public int GetCoverValue()
    {
        int cover = 0;
        cover = Mathf.Max(cover, CheckCoverAtRelativePosition(0, 1));
        cover = Mathf.Max(cover, CheckCoverAtRelativePosition(0, -1));
        cover = Mathf.Max(cover, CheckCoverAtRelativePosition(1, 0));
        cover = Mathf.Max(cover, CheckCoverAtRelativePosition(-1, 0));
        return cover;
    }

    public int GetCoverFromPosition(MapTile position)
    {
        return GetCoverFromPosition(position, currentTile);
    }

    public int GetCoverFromPosition(MapTile shootingPosition, MapTile coverPosition)
    {
        int cover = 0;
        int xDifference = shootingPosition.x - coverPosition.x;
        int zDifference = shootingPosition.z - coverPosition.z;

        if (zDifference < -1)
        {
            cover = Mathf.Max(cover, CheckCoverAtRelativePosition(0, -1, coverPosition));
        }
        else if (zDifference > 1)
        {
            cover = Mathf.Max(cover, CheckCoverAtRelativePosition(0, 1, coverPosition));
        }

        if (xDifference < -1)
        {
            cover = Mathf.Max(cover, CheckCoverAtRelativePosition(-1, 0, coverPosition));
        }
        else if (xDifference > 1)
        {
            cover = Mathf.Max(cover, CheckCoverAtRelativePosition(1, 0, coverPosition));
        }

        return cover;
    }

    public int CheckCoverAtRelativePosition(int x, int z)
    {
        return CheckCoverAtRelativePosition(x, z, currentTile);
    }

    public int CheckCoverAtRelativePosition(int x, int z, MapTile relativeTile)
    {
        MapTile m = MapManager.instance.GetTile(relativeTile.x + x, relativeTile.z + z);

        int wallIndex = MapTile.GetWallIndex(x, z);
        int otherWallIndex = MapTile.GetWallIndex(-x, -z);

        int coverHeight = 0;

        //Walls on the current tile should always give cover
        coverHeight = relativeTile.wallHeights[wallIndex];


        if (m == null)
        {
            //No other tile, just use this tiles cover
            return coverHeight;
        }

        //Get the effective height of the other tile, wall + tile itself
        int otherHeight = m.tileHeight + m.wallHeights[otherWallIndex];

        coverHeight = Mathf.Max(coverHeight, otherHeight - relativeTile.tileHeight);
        if (coverHeight > 2)
        {
            coverHeight = 2;
        }

        return coverHeight;
    }

    public float CalculateAccuracy(MapTile targetTile)
    {
        return CalculateAccuracy(this.currentTile, targetTile);
    }

    public float CalculateAccuracy(MapTile attackerTile, MapTile targetTile)
    {
        float accuracy = unitData.weapon.baseAccuracy;

        //Calculate effect of range on accuracy
        float range = attackerTile.GetDistance(targetTile);
        if (range > unitData.weapon.idealRange)
        {
            accuracy -= unitData.weapon.accuracyDropoffRate * (range - unitData.weapon.idealRange);
        }

        //Calculate effect of cover on attack
        int coverFactor = targetTile.occupant.GetCoverFromPosition(attackerTile);
        int coverMult = 25;//25% per cover height
        if (coverFactor != 0)
        {
            //Apply cover to accuracy
            accuracy -= coverFactor * coverMult;
        }
        return accuracy;
    }

    private void OnValidate()
    {
        ////If not in play mode move the unit
        //if (currentTile != _previousTile && !Application.isPlaying)
        //{
        //    _previousTile = currentTile;

        //    //Remove this unit from the previous tile
        //    if (currentTile != null)
        //    {
        //        currentTile.occupant = null;
        //    }

        //    //Place the unit at the new tile
        //    originalTile = currentTile;
        //    currentTile.occupant = this;
        //    this.transform.position = currentTile.GetSurfacePosition();
        //}

        //mapPosition = new Vector2(currentTile.x, currentTile.z);
    }

    public void ApplyDamage(int amount)
    {
        currentHealth -= amount;
        OnHealthChanged.Invoke();

        //Check for and handle death
        if(currentHealth <= 0)
        {
            HandleDeath();
        }
    }

    public void ApplyHealing(int amount)
    {
        currentHealth += amount;
        if (currentHealth > GetMaxHealth())
        {
            currentHealth = GetMaxHealth();
        }
        OnHealthChanged.Invoke();
    }

    public bool IsAlive()
    {
        return currentHealth > 0;
    }

    public void HandleDeath()
    {
        //Remove this unit from the turn queue if they are there
        BattleManager.instance.turnQueue.Remove(this);
        BattleManager.instance.OnTurnQueueUpdated.Invoke(BattleManager.instance.turnQueue);

        //Remove this unit from its tile
        currentTile.occupant = null;

        //Disable this game object
        this.gameObject.SetActive(false);
    }

    public IEnumerator AnimateMovement(MapTile destination)
    {
        float moveSpeed = 10f;

        while (this.transform.position != destination.GetSurfacePosition())
        {
            //Just translate towards the destination each frame until we are at the correct position
            this.transform.position = Vector3.MoveTowards(this.transform.position, destination.GetSurfacePosition(), Time.deltaTime * moveSpeed);
            yield return new WaitForEndOfFrame();
        }

    }

    public int GetMaxHealth()
    {
        return unitData.baseHealth + unitData.armor.health;
    }

    public int GetMovement()
    {
        return unitData.baseMovementRange + unitData.armor.movement;
    }

    public List<BattleAction> GetUsableActions(MapTile tile)
    {
        List<BattleAction> actions = new List<BattleAction>();

        actions.Add(BattleAction.WaitAction);
        //Add the weapon action
        actions.Add(new BattleAction(unitData.weapon));

        //Grab any actions from items the user has
        unitData.items.ForEach(i => actions.Add(new BattleAction(i)));

        return actions;
    }


    #region Helper Methods
    //These methods are mainly used by AI to assist in decision making and data access
    public int MovementRequiredToGetInRange(Unit target)
    {
        //TODO Actually check range, for now simply do taxi cab distance
        return Mathf.Abs(target.currentTile.x - currentTile.x) + Mathf.Abs(target.currentTile.z - currentTile.z);
    }

    //public int MovementRequiredToReachTile(MapTile tile)
    //{
    //    //TODO Actually check movement logic, for now simply do taxi cab distance
    //    return Mathf.Abs(tile.x - currentTile.x) + Mathf.Abs(tile.z - currentTile.z);
    //}

    public int MovementRequiredToReachTile(MapTile tile, MapTile otherTile)
    {
        //TODO Actually check movement logic, for now simply do taxi cab distance
        return Mathf.Abs(tile.x - otherTile.x) + Mathf.Abs(tile.z - otherTile.z);
    }

    public List<Unit> GetAllEnemies()
    {
        return BattleManager.instance.units.FindAll(unit => unit.team != this.team && unit.IsAlive());
    }
    #endregion

}
