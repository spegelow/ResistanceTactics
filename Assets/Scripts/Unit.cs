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

    public int movementRange;
    
    public int minAttackRange;
    public int maxAttackRange;

    public int minDamage;
    public int maxDamage;

    public float accuracy;

    public int maxHealth;
    public int currentHealth;

    [Header("AI Settings")]
    public bool isAIControlled;
    public UnitAI unitAI;


    [Header("Events")]
    public UnityEvent OnHealthChanged;

    // Start is called before the first frame update
    void Start()
    {
        OnHealthChanged.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveToTile(MapTile newTile)
    {
        if(newTile == currentTile)
        {
            //Nothing needed
            return;
        }

        if(newTile.IsOccupied())
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
    }

    public List<MapTile> GetMoveableTiles()
    {
        return MapManager.instance.GetTilesInRange(currentTile.x, currentTile.z, movementRange);
    }

    public List<MapTile> GetAttackableTiles()
    {
        List<MapTile> targetableTiles = MapManager.instance.GetTilesInRange(currentTile.x, currentTile.z, minAttackRange, maxAttackRange);

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

    private void OnValidate()
    {
        //If not in play mode move the unit
        if(currentTile != _previousTile && !Application.isPlaying)
        {
            _previousTile = currentTile;
            MoveToTile(currentTile);
        }
    }

    public void ApplyDamage(int amount)
    {
        currentHealth -= amount;
        OnHealthChanged.Invoke();

        //TODO Check for death

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
        return BattleManager.instance.units.FindAll(unit => unit.team != this.team);
    }
    #endregion

}
