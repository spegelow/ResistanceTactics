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
        statsUI.text = "" + unit.unitName + " " + unit.currentHealth + "/" + unit.GetMaxHealth() + "\nMovement " + unit.GetMovement() + "\n\n" + unit.armor.itemName + "\nArmor " + unit.armor.armor + "\nDodge Chance " + unit.armor.dodge + "\n\n" + unit.weapon.itemName + "\nMin Range " + unit.weapon.minAttackRange + "\nMax Range " + unit.weapon.maxAttackRange + "\nMin Damage " + unit.weapon.minDamage + "\nMax Damage " + unit.weapon.maxDamage + "\nBase Accuracy " + unit.weapon.baseAccuracy + "\nIdeal Range " + unit.weapon.idealRange + "\nAccuracy Dropoff Rate " + unit.weapon.accuracyDropoffRate;
    }
}
