using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Scriptable Objects/Equipment/Weapon", order = 1)]
public class Weapon : ScriptableObject
{
    public string itemName;

    public int minAttackRange;
    public int maxAttackRange;

    public int minDamage;
    public int maxDamage;

    public float accuracy;
}
