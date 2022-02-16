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


    public enum InputState { MovementSelection, ActionSelection, TargetSelection, IgnoreInput}
    public InputState inputState;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;

        selectableTiles = new List<MapTile>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            switch (inputState)
            {
                case InputState.MovementSelection:
                    //No effect when selecting movement
                    break;

                case InputState.ActionSelection:
                    //Undo the unit's movement
                    //TODO
                    break;

                case InputState.TargetSelection:
                    //Undo the action selection and show the action list again
                    //TODO
                    break;
            }
        }
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
        switch (inputState)
        {
            case InputState.MovementSelection:
                //Move the unit to the selected tile if it is valid
                if (currentUnit != null)
                {
                    //Validate the clicked tile
                    if (selectableTiles.Contains(tile))
                    {
                        selectableTiles.Clear();
                        currentAction(currentUnit, tile);
                    }
                }
                break;

            case InputState.ActionSelection:
                //Do nothing? They need to select an action first
                //TODO
                break;

            case InputState.TargetSelection:
                //Activate the currentAction targeting the selected tile if it is valid
                if (currentUnit != null)
                {
                    //Validate the clicked tile
                    if (selectableTiles.Contains(tile) && currentUnit.IsValidTarget(tile))
                    {
                        selectableTiles.Clear();
                        currentAction(currentUnit, tile);
                    }
                }
                break;
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

    public void SetupActionSelection()
    {
        inputState = InputState.ActionSelection;

        //For now... I guess just skip this and say an action was selected??
        SetCurrentUnit(BattleManager.instance.turnQueue[0]);
        SetSelectableTiles(BattleManager.instance.turnQueue[0].GetAttackableTiles());
        currentAction = BattleManager.instance.ResolveAttack; //Set the current action to MoveUnit

        inputState = InputState.TargetSelection;
    }
}
