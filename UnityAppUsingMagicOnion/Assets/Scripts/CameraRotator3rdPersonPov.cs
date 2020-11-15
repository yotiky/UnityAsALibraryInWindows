using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotator3rdPersonPov : MonoBehaviour
{
    public GameObject playerObject;
    public Vector2 rotationSpeed;
    public float zoomSpeed;
    public bool reverse;

    private Camera mainCamera;
    private Vector2 lastMousePosition;
    private Vector3 newAngle = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            if (!reverse)
            {
                var x = (lastMousePosition.x - Input.mousePosition.x);
                var y = (Input.mousePosition.y - lastMousePosition.y);

                if (Mathf.Abs(x) < Mathf.Abs(y))
                    x = 0;
                else
                    y = 0;

                newAngle.x = x * rotationSpeed.x;
                newAngle.y = y * rotationSpeed.y;

                mainCamera.transform.RotateAround(playerObject.transform.position, Vector3.up, newAngle.x);
                mainCamera.transform.RotateAround(playerObject.transform.position, transform.right, newAngle.y);
                lastMousePosition = Input.mousePosition;
            }
            else
            {
                var x = (Input.mousePosition.x - lastMousePosition.x);
                var y = (lastMousePosition.y - Input.mousePosition.y);

                if (Mathf.Abs(x) < Mathf.Abs(y)) 
                    x = 0; 
                else 
                    y = 0; 

                newAngle.x = x * rotationSpeed.x;
                newAngle.y = y * rotationSpeed.y;

                mainCamera.transform.RotateAround(playerObject.transform.position, Vector3.up, newAngle.x);
                mainCamera.transform.RotateAround(playerObject.transform.position, transform.right, newAngle.y);
                lastMousePosition = Input.mousePosition;
            }
        }


        var scroll = Input.mouseScrollDelta.y;
        mainCamera.transform.position += mainCamera.transform.forward * scroll * zoomSpeed;
    }
}
