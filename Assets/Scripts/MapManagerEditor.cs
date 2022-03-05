using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(MapManager))]
public class MapManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //Draw the base inspector
        base.OnInspectorGUI();

        //Get the inspected object
        MapManager targetManager = (MapManager) target;

        //Display our buttons
        if (GUILayout.Button("Clear Map"))
        {
            targetManager.ClearMap();
        }

        if (GUILayout.Button("Generate New Map"))
        {
            targetManager.GenerateMap(targetManager.mapWidth, targetManager.mapHeight);
        }

        if (GUILayout.Button("Save Map Geometry"))
        {
            targetManager.SaveMap();
        }

        if (GUILayout.Button("Load Map Geometry"))
        {
            targetManager.LoadMap();
        }

        //Actually save the map manager in scene
        if (GUI.changed)
        {
            EditorUtility.SetDirty(targetManager);
            EditorSceneManager.MarkSceneDirty(targetManager.gameObject.scene);
        }
    }
}
