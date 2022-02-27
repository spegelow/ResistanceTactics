using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float rotationSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Rotate Counterclockwise
        if(Input.GetKey(KeyCode.Q))
        {
            this.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }

        //Rotate Clockwise
        if (Input.GetKey(KeyCode.E))
        {
            this.transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
        }
    }
}
