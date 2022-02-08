using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevUtilities : MonoBehaviour
{
    public MapManager mapManager;
    public GameObject cameraPivot;
    public void RecenterCamera()
    {
        Vector3 position = new Vector3((mapManager.mapWidth - 1) / 2f, 0, (mapManager.mapHeight - 1) / 2f);
        cameraPivot.transform.position = position;
    }
}
