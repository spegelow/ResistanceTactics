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
        return MapManager.instance.GetTilesInRange(currentTile.x, currentTile.z, minAttackRange, maxAttackRange);
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
}
