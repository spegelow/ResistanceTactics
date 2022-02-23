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
        attackUI.text = "" + unit.unitName + " " + unit.currentHealth + "/" + unit.maxHealth + "\n" + (BattleManager.GetCurrentUnit().accuracy * 100) + "\n" + BattleManager.GetCurrentUnit().minDamage + "-" + BattleManager.GetCurrentUnit().maxDamage;
    }

    public void HideAttackUI()
    {
        attackUI.gameObject.SetActive(false);
    }
}
