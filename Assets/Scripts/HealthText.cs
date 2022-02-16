using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMpro; 

public class HealthText : MonoBehaviour
{
    void Start ()
    {

    }

    public void UpdateUI(Unit unit)
    {
        HealthUI.text = unit.currentHealth;
    }
}
