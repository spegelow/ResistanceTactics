using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData", menuName = "Scriptable Objects/Map Data", order = 1)]
public class MapData : ScriptableObject
{
    public int width;
    public int height;
    public List<int> tileHeights;
    public List<int[]> wallHeights;
    
    [System.Serializable]
    public struct UnitSpawn
    {
        public UnitData unit;
        public Vector2 point;
    }

    public List<UnitSpawn> playerUnits;
    public List<UnitSpawn> enemyUnits;
}
