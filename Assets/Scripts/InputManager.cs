using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    public GameObject cursor;
    public MapTile currentCursorTile;

    List<MapTile> selectableTiles;
    Unit currentUnit;

    public UnityEvent<Unit> OnUnitHovered;
    public UnityEvent<Unit> OnUnitHoveredTargeting;
    public UnityEvent OnTargetingStart;
    public UnityEvent OnTargetingEnd;


    public GameObject actionPanel;

    public delegate IEnumerator ActionDelegate(Unit unit, MapTile targetTile);
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
                    actionPanel.SetActive(false);
                    BattleManager.instance.UndoMovement();
                    break;

                case InputState.TargetSelection:
                    //Undo the action selection and show the action list again
                    SetupActionSelection();
                    OnTargetingEnd.Invoke();
                    break;
            }
        }
    }

    public void TileHovered(MapTile tile)
    {
        
        if (instance.currentCursorTile != tile)
        {
            instance.MoveCursor(tile);

            if(tile.occupant != null)
            {
                if (inputState == InputState.TargetSelection)
                {
                    OnUnitHoveredTargeting.Invoke(tile.occupant);
                }

                OnUnitHovered.Invoke(tile.occupant);
            }
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
                        inputState = InputState.IgnoreInput;
                        StartCoroutine(currentAction(currentUnit, tile));
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
                        OnTargetingEnd.Invoke();
                        selectableTiles.Clear();
                        StartCoroutine(currentAction(currentUnit, tile));
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

    public void SetSelectableTiles(List<MapTile> tiles, Color color)
    {
        selectableTiles = tiles;

        //Set highlighting for tiles
        MapManager.instance.ClearTileHighlights();
        tiles.ForEach(tile => tile.SetHighlight(color));
    }

    public void SetupActionSelection()
    {
        actionPanel.SetActive(true);
        MapManager.instance.ClearTileHighlights();
        inputState = InputState.ActionSelection;
    }

    public void ActionButtonClicked(string actionName)
    {
        if(inputState != InputState.ActionSelection)
        {
            //How did we even click the button then???
            Debug.LogError("Cannot select action when not in action selection state");
            return;
        }

        if(actionName == "Wait")
        {
            //No target is needed for wait, so just call the battle manager
            inputState = InputState.IgnoreInput;
            BattleManager.instance.Wait(currentUnit, null);

        }
        else if(actionName == "Attack")
        {
            SetCurrentUnit(BattleManager.instance.turnQueue[0]);
            SetSelectableTiles(BattleManager.instance.turnQueue[0].GetAttackableTiles(), Color.red);
            currentAction = BattleManager.instance.ResolveAttack; //Set the current action to MoveUnit
            inputState = InputState.TargetSelection;
            OnTargetingStart.Invoke();
        }

        actionPanel.SetActive(false);
    }
}
