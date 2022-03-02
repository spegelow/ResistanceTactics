using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Armor", menuName = "Scriptable Objects/Equipment/Armor", order = 1)]
public class Armor : ScriptableObject
{
    public string itemName;

    public int health;
    public int armor;
    public int movement;
    public int dodge;

}
