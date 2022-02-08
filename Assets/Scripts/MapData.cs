using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData", menuName = "ScriptableObjects/Map Data", order = 1)]
public class MapData : ScriptableObject
{
    public int width;
    public int height;
    public List<int> tileHeights;
}
