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
        statsUI.gameObject.SetActive(false);
    }

    public void ShowStatsUI()
    {
        statsUI.gameObject.SetActive(true);
    }


    public void HideStatsUI()
    {
        statsUI.gameObject.SetActive(false);
    }

    public void UpdateUI(Unit unit)
    {
        statsUI.text = "" + unit.unitData.unitName + " " + unit.currentHealth + "/" + unit.GetMaxHealth() + "\nMovement " + unit.GetMovement() + "\n\n" + unit.unitData.armor.itemName + "\nArmor " + unit.unitData.armor.armor + "\nDodge Chance " + unit.unitData.armor.dodge + "\n\n" + unit.unitData.weapon.itemName + "\nMin Range " + unit.unitData.weapon.minAttackRange + "\nMax Range " + unit.unitData.weapon.maxAttackRange + "\nMin Damage " + unit.unitData.weapon.minDamage + "\nMax Damage " + unit.unitData.weapon.maxDamage + "\nBase Accuracy " + unit.unitData.weapon.baseAccuracy + "\nIdeal Range " + unit.unitData.weapon.idealRange + "\nAccuracy Dropoff Rate " + unit.unitData.weapon.accuracyDropoffRate;
    }
}
