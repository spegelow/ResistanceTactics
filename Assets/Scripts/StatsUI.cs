using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatsUI : MonoBehaviour
{
    public TMP_Text statsUI;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void UpdateUI(Unit unit)
    {
        statsUI.text = "" + unit.unitName + "\n Movement " + unit.movementRange + "\n Min Range" + unit.minAttackRange + "\n Max Range" + unit.maxAttackRange + "\n Min Damage" + unit.minDamage + "\n Max Damage" + unit.maxDamage + "\n Accuracy" + unit.accuracy;
    }
}
