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
        statsUI.text = "" + unit.unitName + " " + unit.currentHealth + "/" + unit.GetMaxHealth() + "\nMovement " + unit.GetMovement() + "\n" + BattleManager.GetCurrentUnit().armor.itemName + "\nArmor " + BattleManager.GetCurrentUnit().armor.armor + "\nDodge Chance " + BattleManager.GetCurrentUnit().armor.dodge + "\n" + BattleManager.GetCurrentUnit().weapon.itemName + "\nMin Range " + BattleManager.GetCurrentUnit().weapon.minAttackRange + "\nMax Range " + BattleManager.GetCurrentUnit().weapon.maxAttackRange + "\nMin Damage " + BattleManager.GetCurrentUnit().weapon.minDamage + "\nMax Damage " + BattleManager.GetCurrentUnit().weapon.MaxDamage + "\nBase Accuracy " + unit.weapon.baseAccuracy + "\nIdeal Range " + BattleManager.GetCurrentUnit().weapon.idealRange + "\nAccuracy Dropoff Rate " + BattleManager.GetCurrentUnit().weapon.accuracyDropoffRate;
    }
}
