using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AttackPreviewUI : MonoBehaviour
{

    public TMP_Text attackUI;

    // Start is called before the first frame update
    void Start()
    {
        HideAttackUI();
    }

    public void ShowAttackUI(Unit unit)
    {
        UpdateAttackUI(unit);

        attackUI.gameObject.SetActive(true);
    }

    public void UpdateAttackUI(Unit unit)
    {
        attackUI.text = "" + unit.unitName + " " + unit.currentHealth + "/" + unit.GetMaxHealth() + "\n" + (BattleManager.GetCurrentUnit().weapon.accuracy * 100) + "\n" + BattleManager.GetCurrentUnit().weapon.minDamage + "-" + BattleManager.GetCurrentUnit().weapon.maxDamage;
    }

    public void HideAttackUI()
    {
        attackUI.gameObject.SetActive(false);
    }
}
