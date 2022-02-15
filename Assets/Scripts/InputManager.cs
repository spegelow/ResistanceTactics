using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    public GameObject cursor;
    public MapTile currentCursorTile;

    List<MapTile> selectableTiles;
    Unit currentUnit;

    public delegate void ActionDelegate(Unit unit, MapTile targetTile);
    public ActionDelegate currentAction;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;

        selectableTiles = new List<MapTile>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TileHovered(MapTile tile)
    {
        
        if (instance.currentCursorTile != tile)
        {
            instance.MoveCursor(tile);
        }
    }

    public void TileClicked(MapTile tile)
    {
        if (currentUnit != null)
        {
            //Validate the clicked tile
            //TODO Generalize this code for any action, not just movement
            if (selectableTiles.Contains(tile))
            {
                selectableTiles.Clear();
                currentAction(currentUnit, tile);
            }
        }
    }


    void MoveCursor(MapTile tile)
    {
        //Move the cursor to the hovered tile and update any UI as necessary
        currentCursorTile = tile; 
        cursor.transform.position = tile.GetSurfacePosition();
        

        //TODO UPDATE UI
    }

    public void SetCurrentUnit(Unit unit)
    {
        currentUnit = unit;
    }

    public void SetSelectableTiles(List<MapTile> tiles)
    {
        selectableTiles = tiles;

        //Set highlighting for tiles
        MapManager.instance.ClearTileHighlights();
        tiles.ForEach(tile => tile.SetHighlight(true));
    }
}
