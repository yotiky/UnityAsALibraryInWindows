using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotator1stPersonPov : MonoBehaviour
{
    public GameObject playerObject;
    public Vector2 rotationSpeed;
    public float zoomSpeed;
    public bool reverse;

    private Camera mainCamera;
    private Vector2 lastMousePosition;
    private Vector2 newAngle = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            newAngle = mainCamera.transform.localEulerAngles;
            lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            if(!reverse)
            {
                newAngle.y -= (lastMousePosition.x - Input.mousePosition.x) * rotationSpeed.y;
                newAngle.x -= (Input.mousePosition.y - lastMousePosition.y) * rotationSpeed.x;

                mainCamera.transform.localEulerAngles = newAngle;
                lastMousePosition = Input.mousePosition;
            }
            else
            {
                newAngle.y -= (Input.mousePosition.x - lastMousePosition.x) * rotationSpeed.y;
                newAngle.x -= (lastMousePosition.y - Input.mousePosition.y) * rotationSpeed.x;

                mainCamera.transform.localEulerAngles = newAngle;
                lastMousePosition = Input.mousePosition;
            }
        }

        var scroll = Input.mouseScrollDelta.y;
        mainCamera.transform.position += mainCamera.transform.forward * scroll * zoomSpeed;
    }
}
