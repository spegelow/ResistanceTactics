using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Unit : MonoBehaviour
{
    public MapTile currentTile;
    public MapTile originalTile;//The tile this unit began movement on, used for undoing movement
    MapTile _previousTile;

    public string unitName;

    public int team;

    public int baseMovementRange;

    public int maxVerticalMovement = 1;

    public Weapon weapon;
    public Armor armor;

    public int baseHealth;
    public int currentHealth;

    [Header("AI Settings")]
    public bool isAIControlled;
    public UnitAI unitAI;


    [Header("Events")]
    public UnityEvent OnHealthChanged;
    public UnityEvent OnUnitMoved;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = GetMaxHealth();
        OnHealthChanged.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveToTile(MapTile newTile)
    {
        //if(newTile == currentTile)
        //{
        //    //Nothing needed
        //    return;
        //}

        if(newTile.IsOccupied() && newTile.occupant != this)
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
        List<MapTile> targetableTiles = MapManager.instance.GetTilesInRange(currentTile.x, currentTile.z, weapon.minAttackRange, weapon.maxAttackRange);

        //Only return the tiles in line of sight
        return targetableTiles.FindAll(tile => CanSeeTile(tile));
    
    }

    public bool CanSeeTile(MapTile tile)
    {
        Vector3 attackOffset = new Vector3(0, 1.5f, 0);//Used to check line of sight from roughly the units face
        return !Physics.Linecast(this.currentTile.GetSurfacePosition() + attackOffset, tile.GetSurfacePosition() + attackOffset);
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
        int cover = 0;
        int xDifference = position.x - currentTile.x;
        int zDifference = position.z - currentTile.z;

        if(zDifference<-1)
        {
            cover = Mathf.Max(cover, CheckCoverAtRelativePosition(0,-1));
        }
        else if(zDifference>1)
        {
            cover = Mathf.Max(cover, CheckCoverAtRelativePosition(0, 1));
        }

        if (xDifference < -1)
        {
            cover = Mathf.Max(cover, CheckCoverAtRelativePosition(-1, 0));
        }
        else if (xDifference > 1)
        {
            cover = Mathf.Max(cover, CheckCoverAtRelativePosition(1, 0));
        }

        return cover;
    }

    public int CheckCoverAtRelativePosition(int x, int z)
    {
        MapTile m = MapManager.instance.GetTile(currentTile.x + x, currentTile.z + z);

        if(m==null)
        {
            //No tile at position, no cover
            return 0;
        }

        if(m.tileHeight <= currentTile.tileHeight)
        {
            //Same height or lower, no cover
            return 0;
        }
        else if(m.tileHeight == currentTile.tileHeight + 1)
        {
            //One tile higher, half cover
            return 1;
        }
        else
        {
            //Two or more higher, full cover
            return 2;
        }
    }

    public float CalculateAccuracy(MapTile tile)
    {
        float accuracy = weapon.baseAccuracy;

        //Calculate effect of range on accuracy
        float range = this.currentTile.GetDistance(tile);
        if(range > weapon.idealRange)
        {
            accuracy -= weapon.accuracyDropoffRate * (range - weapon.idealRange);
        }

        //Calculate effect of cover on attack
        int coverFactor = tile.occupant.GetCoverFromPosition(this.currentTile);
        int coverMult = 25;//25% per cover height
        if(coverFactor != 0)
        {
            //Apply cover to accuracy
            accuracy -= coverFactor * coverMult;
        }
        return accuracy;
    }

    //private void OnValidate()
    //{
    //    //If not in play mode move the unit
    //    if(currentTile != _previousTile && !Application.isPlaying)
    //    {
    //        _previousTile = currentTile;

    //        //Remove this unit from the previous tile
    //        if (currentTile != null)
    //        {
    //            currentTile.occupant = null;
    //        }

    //        //Place the unit at the new tile
    //        originalTile = currentTile;
    //        currentTile.occupant = this;
    //        this.transform.position = currentTile.GetSurfacePosition();
    //    }
    //}

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
        return baseHealth + armor.health;
    }

    public int GetMovement()
    {
        return baseMovementRange + armor.movement;
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
