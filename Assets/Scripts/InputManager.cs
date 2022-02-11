using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    public GameObject cursor;
    public MapTile currentCursorTile;

    public Unit unit;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void TileHovered(MapTile tile)
    {
        
        if (instance.currentCursorTile != tile)
        {
            instance.MoveCursor(tile);
        }
    }

    public static void TileClicked(MapTile tile)
    {
        
        instance.unit.MoveToTile(tile);


        MapManager.instance.ClearTileHighlights();
        instance.unit.GetMoveableTiles().ForEach(tile => tile.SetHighlight(true));
    }


    void MoveCursor(MapTile tile)
    {
        //Move the cursor to the hovered tile and update any UI as necessary
        currentCursorTile = tile; 
        cursor.transform.position = tile.GetSurfacePosition();
        

        //TODO UPDATE UI
    }
}
