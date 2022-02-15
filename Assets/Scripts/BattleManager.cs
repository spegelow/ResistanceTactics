using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;

    public List<Unit> turnQueue;
    public List<Unit> units;

    public void Awake()
    {
        instance = this;
    }

    public void StartBattle()
    {
        //Build the turn queue
        turnQueue = new List<Unit>();
        units.ForEach(unit => turnQueue.Add(unit));

        //Start the first turn
        StartTurn();
    }

    public void StartTurn()
    {
        //Show current units movement range
        InputManager.instance.SetCurrentUnit(turnQueue[0]);
        InputManager.instance.SetSelectableTiles(turnQueue[0].GetMoveableTiles());
        InputManager.instance.currentAction = MoveUnit; //Set the current action to MoveUnit
    }

    public void EndTurn()
    {
        //Remove the current unit from the queue
        turnQueue.RemoveAt(0);

        //Check if there are any units left for this round
        if(turnQueue.Count > 0)
        {
            //There are units left, simply start the next turn
            StartTurn();
        }
        else
        {
            //No units were left, so start the next round
            //TODO END ROUND
            //TEMP simply rebuild the queue and start a new turn
            turnQueue = new List<Unit>();
            units.ForEach(unit => turnQueue.Add(unit));
            StartTurn();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        StartBattle();
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void MoveUnit(Unit unit, MapTile targetTile)
    {
        unit.MoveToTile(targetTile);

        unit = null;
        MapManager.instance.ClearTileHighlights();
        
        //TODO Set up action input instead of just ending the turn
        BattleManager.instance.EndTurn();
    }
}
