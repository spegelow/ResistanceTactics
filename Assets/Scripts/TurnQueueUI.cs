using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TurnQueueUI : MonoBehaviour
{

    public TMP_Text turns;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnTurnQueueUpdated(List<Unit>turnQueue)
    {
        string toDisplay = "";
        turnQueue.ForEach(unit => toDisplay += unit.unitName + "\n");
        turns.text = toDisplay;
    }
}
