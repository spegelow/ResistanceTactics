using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;

    public List<Unit> turnQueue;
    public bool randomizeTurnOrderOnStart;
    public List<Unit> units;

    public UnityEvent<List<Unit>> OnTurnQueueUpdated;

    public GameObject floatingDamageTextPrefab;
    Vector3 damageTextOffset = new Vector3(0, 1, 0);

    public GameObject endGameUIPanel;
    public TMP_Text endGameMessage;

    public void Awake()
    {
        instance = this;
    }

    public void StartBattle()
    {
        //If necessary, randomize the unit list order
        if (randomizeTurnOrderOnStart)
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
        if (turnQueue.Count > 0)
        {
            turnQueue.RemoveAt(0);
        }

        if(IsBattleDone())
        {
            HandleBattleEnd();
            return;
        }

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
            units.ForEach(unit =>
            {
                if (unit.IsAlive())
                {
                    turnQueue.Add(unit);
                }
            });
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

    public IEnumerator MoveUnit(Unit unit, MapTile targetTile)
    {
        MapManager.instance.ClearTileHighlights();

        //Animate the units movement
        yield return StartCoroutine(unit.AnimateMovement(targetTile));

        //TODO Add the ability to skip movement animations
        unit.MoveToTile(targetTile);

        //unit = null;????Why was this a thing

        //TODO Set up action input instead of just ending the turn
        InputManager.instance.SetupActionSelection();
    }

    public void AIMoveUnit(Unit unit, MapTile targetTile)
    {
        unit.MoveToTile(targetTile);
    }

    public IEnumerator ResolveAttack(Unit attacker, MapTile targetTile)
    {
        yield return new WaitForEndOfFrame();
        //Do an accuracy check
        float aimCheck = Random.value;
        if(aimCheck > attacker.accuracy)
        {
            CreateFloatingText(targetTile.occupant, "MISS!");
            yield return new WaitForSeconds(1);

        }
        else
        {
            //The attack hit, so let's determine damage
            int baseDamage = Random.Range(attacker.minDamage, attacker.maxDamage + 1);

            //Apply the damage to the target (if there is one?)
            CreateDamageText(targetTile.occupant, baseDamage);
            yield return new WaitForSeconds(1);
            targetTile.occupant?.ApplyDamage(baseDamage);
        }


        EndTurn();
    }

    public void CreateDamageText(Unit target, int damage)
    {
        GameObject newObject = GameObject.Instantiate(floatingDamageTextPrefab, target.transform.position + damageTextOffset, Quaternion.identity);
        FloatingText text = newObject.GetComponent<FloatingText>();

        text.InitializeDamageText(damage);
    }

    public void CreateFloatingText(Unit target, string message)
    {
        GameObject newObject = GameObject.Instantiate(floatingDamageTextPrefab, target.transform.position + damageTextOffset, Quaternion.identity);
        FloatingText text = newObject.GetComponent<FloatingText>();

        text.InitializeFloatingText(message);
    }

    public static Unit GetCurrentUnit()
    {
        return instance.turnQueue[0];
    }

    public void Wait(Unit unit, MapTile targetTile)
    {
        EndTurn();
    }

    public bool IsBattleDone()
    {
        int remainingTeam = -1;
        bool isBattleDone = true;
        units.ForEach(unit =>
        {
            //Is this unit alive?
            if (isBattleDone && unit.IsAlive())
            {
                //If we haven't found another team left yet, save this team
                if (remainingTeam == -1)
                {
                    remainingTeam = unit.team;
                }
                else if (remainingTeam != unit.team)
                {
                    //We found two different teams so battle isn't done
                    isBattleDone = false;
                }
            }
        });

        return isBattleDone;
    }

    public void HandleBattleEnd()
    {
        int winningTeam = -1;

        //Should only be one team left, so just find the first unit that is alive and grab its team
        winningTeam = units.Find(unit => unit.IsAlive()).team;

        //TEMP For now assume the player is team 0
        //Enable the end game panel and message
        endGameUIPanel.SetActive(true);
        endGameMessage.text = (winningTeam == 0) ? "~ Victory ~" : "~ Defeat ~";

    }

    public void RestartBattle()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
