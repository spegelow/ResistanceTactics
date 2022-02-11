using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    public GameObject cursor;
    public MapTile currentCursorTile;

    List<MapTile> moveableTiles;
    Unit currentUnit;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;

        moveableTiles = new List<MapTile>();
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
            if (moveableTiles.Contains(tile))
            {
                currentUnit.MoveToTile(tile);

                currentUnit = null;
                MapManager.instance.ClearTileHighlights();
                moveableTiles.Clear();

                BattleManager.instance.EndTurn();
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

    public void SetMoveableTiles(List<MapTile> tiles)
    {
        moveableTiles = tiles;

        //Set highlighting for tiles
        MapManager.instance.ClearTileHighlights();
        tiles.ForEach(tile => tile.SetHighlight(true));
    }
}
