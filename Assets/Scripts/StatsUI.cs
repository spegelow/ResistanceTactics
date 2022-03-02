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
        statsUI.text = "" + unit.unitName + "\n Movement " + unit.GetMovement() + "\n Min Range " + unit.weapon.minAttackRange + "\n Max Range " + unit.weapon.maxAttackRange + "\n Min Damage " + unit.weapon.minDamage + "\n Max Damage " + unit.weapon.maxDamage + "\n Accuracy " + unit.weapon.accuracy;
    }
}
