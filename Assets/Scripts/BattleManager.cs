using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;

    public List<Unit> turnQueue;
    public bool randomizeUnitsOnStart;
    public List<Unit> units;

    public UnityEvent<List<Unit>> OnTurnQueueUpdated;

    public void Awake()
    {
        instance = this;
    }

    public void StartBattle()
    {
        //If necessary, randomize the unit list order
        if (randomizeUnitsOnStart)
        {
            units.Sort((a, b) => Random.Range(-1, 2));
        }
        //Build the turn queue
        turnQueue = new List<Unit>();
        units.ForEach(unit => turnQueue.Add(unit));
        OnTurnQueueUpdated.Invoke(turnQueue);
        //Start the first turn
        StartTurn();
    }

    public void StartTurn()
    {
        //Show current units movement range
        Unit currentUnit = turnQueue[0];
        if (currentUnit.isAIControlled)
        {
            StartCoroutine(currentUnit.unitAI.BeginTurn());
        }
        else //Player Controlled
        {
            InputManager.instance.SetCurrentUnit(currentUnit);
            InputManager.instance.SetSelectableTiles(currentUnit.GetMoveableTiles(), Color.green);
            InputManager.instance.currentAction = MoveUnit; //Set the current action to MoveUnit
            InputManager.instance.inputState = InputManager.InputState.MovementSelection;
        }
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


        OnTurnQueueUpdated.Invoke(turnQueue);
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

    public void UndoMovement()
    {
        //Undo the units movement
        turnQueue[0].UndoMovement();

        //Resetup movement selection
        InputManager.instance.SetCurrentUnit(turnQueue[0]);
        InputManager.instance.SetSelectableTiles(turnQueue[0].GetMoveableTiles(), Color.green);
        InputManager.instance.currentAction = MoveUnit; //Set the current action to MoveUnit
        InputManager.instance.inputState = InputManager.InputState.MovementSelection;
    }

    public void MoveUnit(Unit unit, MapTile targetTile)
    {
        unit.MoveToTile(targetTile);

        //unit = null;????Why was this a thing
        MapManager.instance.ClearTileHighlights();

        //TODO Set up action input instead of just ending the turn
        InputManager.instance.SetupActionSelection();
    }

    public void AIMoveUnit(Unit unit, MapTile targetTile)
    {
        unit.MoveToTile(targetTile);
    }

    public void ResolveAttack(Unit attacker, MapTile targetTile)
    {
        //Do an accuracy check
        float aimCheck = Random.value;
        if(aimCheck > attacker.accuracy)
        {
            //The attack missed
            Debug.Log("The attack missed");
            
        }
        else
        {
            //The attack hit, so let's determine damage
            int baseDamage = Random.Range(attacker.minDamage, attacker.maxDamage + 1);

            //Apply the damage to the target (if there is one?)
            targetTile.occupant?.ApplyDamage(baseDamage);
        }


        EndTurn();
    }

    public static Unit GetCurrentUnit()
    {
        return instance.turnQueue[0];
    }

    public void Wait(Unit unit, MapTile targetTile)
    {
        EndTurn();
    }
}
