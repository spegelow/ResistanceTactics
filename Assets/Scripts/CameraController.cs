using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float rotationSpeed;
    public float moveSpeed;
    public float zoomSpeed;

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

        Vector3 moveDirection = Vector3.zero;

        if(Input.GetKey(KeyCode.W))
        {
            moveDirection.z += 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveDirection.z += -1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveDirection.x += -1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDirection.x += 1;
        }

        moveDirection = Vector3.ProjectOnPlane(moveDirection, Vector3.up);
        moveDirection = Quaternion.AngleAxis(this.transform.rotation.eulerAngles.y, Vector3.up) * moveDirection;

        //Move the camera relative to its facing and the xz-plane
        this.transform.position += moveDirection.normalized * Time.deltaTime * moveSpeed;


        float zoomFactor = Input.mouseScrollDelta.y * zoomSpeed;
        Camera.main.orthographicSize += zoomFactor;
    }
}
