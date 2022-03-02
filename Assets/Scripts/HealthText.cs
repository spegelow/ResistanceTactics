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

        if (unit.GetCoverValue() == 1)
        {
            healthUI.text += "\nHalf Cover";
        }
        else if (unit.GetCoverValue() == 2)
        {
            healthUI.text += "\nFull Cover";
        }
    }
}
