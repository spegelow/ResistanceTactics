using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryItem", menuName = "Scriptable Objects/Equipment/InventoryItem", order = 1)]
public class InventoryItem : ScriptableObject
{
    public string itemName;

    public int minAttackRange;
    public int maxAttackRange;

    public int minDamage;
    public int maxDamage;

    public float baseAccuracy;
    public int idealRange;
    public float accuracyDropoffRate;
}
