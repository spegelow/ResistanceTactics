using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "Scriptable Objects/Unit Data", order = 1)]
public class UnitData : ScriptableObject
{
    public string unitName;

    public int baseMovementRange;
    public int maxVerticalMovement = 1;

    public Weapon weapon;
    public Armor armor;

    public int baseHealth;
}
