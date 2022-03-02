using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class HealthText : MonoBehaviour
{
    public TMP_Text healthUI;

    void Start ()
    {

    }

    public void UpdateUI(Unit unit)
    {
        healthUI.text = "" + unit.currentHealth + "/" + unit.GetMaxHealth();
    }
}
