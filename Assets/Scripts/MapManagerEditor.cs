//C# Example (LookAtPointEditor.cs)
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapManager))]
public class MapManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MapManager targetManager = (MapManager) target;

        if (GUILayout.Button("Clear Map"))
        {
            targetManager.ClearMap();
        }

        if (GUILayout.Button("Generate New Map"))
        {
            targetManager.GenerateMap(targetManager.mapWidth, targetManager.mapHeight);
        }
}
}
