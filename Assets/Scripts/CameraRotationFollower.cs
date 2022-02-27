using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotationFollower : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //TODO Don't do this every frame, only while camera is rotating
        Vector3 forward = Camera.main.transform.forward;
        forward.y = 0f;
        transform.rotation = Quaternion.LookRotation(forward);
    }
}
