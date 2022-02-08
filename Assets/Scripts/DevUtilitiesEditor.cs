using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DevUtilities))]
public class DevUtilitiesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //Draw the base inspector
        base.OnInspectorGUI();

        //Get the inspected object
        DevUtilities util = (DevUtilities)target;

        //Display our buttons
        if (GUILayout.Button("Recenter Camera"))
        {
            util.RecenterCamera();
        }
    }
}
