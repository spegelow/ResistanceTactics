using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public MapTile currentTile;
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

    // Start is called before the first frame update
    void Start()
    {
        
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
        currentTile = newTile;
        newTile.occupant = this;
        this.transform.position = newTile.GetSurfacePosition();
    }

    public List<MapTile> GetMoveableTiles()
    {
        return MapManager.instance.GetTilesInRange(currentTile.x, currentTile.z, movementRange);
    }

    public List<MapTile> GetAttackableTiles()
    {
        return MapManager.instance.GetTilesInRange(currentTile.x, currentTile.z, minAttackRange, maxAttackRange);
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
}
