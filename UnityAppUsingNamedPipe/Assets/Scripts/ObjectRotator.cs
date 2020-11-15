using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRotator : MonoBehaviour
{

    public GameObject playerObject;
    public Vector2 rotationSpeed;
    public float zoomSpeed;
    public bool reverse;

    private Camera mainCamera;
    private Vector2 lastMousePosition;
    private Vector3 newAngle = Vector3.zero;
    public RotatableAxis RotatableAxis { get; set; } = RotatableAxis.XY;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            newAngle = mainCamera.transform.localEulerAngles;
            lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            if (!reverse)
            {
                var x = (Input.mousePosition.y - lastMousePosition.y);
                var y = (lastMousePosition.x - Input.mousePosition.x); 

                if (RotatableAxis == RotatableAxis.X)
                    y = 0;
                else if (RotatableAxis == RotatableAxis.Y)
                    x = 0;
                else if (RotatableAxis == RotatableAxis.None)
                    x = y = 0;
                else if (Mathf.Abs(x) < Mathf.Abs(y))
                    x = 0;
                else
                    y = 0;

                newAngle.x = x * rotationSpeed.x;
                newAngle.y = y * rotationSpeed.y;

                playerObject.transform.Rotate(newAngle);
                lastMousePosition = Input.mousePosition;
            }
            else
            {
                var x = (lastMousePosition.y - Input.mousePosition.y);
                var y = (Input.mousePosition.x - lastMousePosition.x); 

                if (RotatableAxis == RotatableAxis.X)
                    y = 0;
                else if (RotatableAxis == RotatableAxis.Y)
                    x = 0;
                else if (RotatableAxis == RotatableAxis.None)
                    x = y = 0;
                else if (Mathf.Abs(x) < Mathf.Abs(y))
                    x = 0;
                else
                    y = 0;

                newAngle.x = x * rotationSpeed.x;
                newAngle.y = y * rotationSpeed.y;

                playerObject.transform.Rotate(newAngle);
                lastMousePosition = Input.mousePosition;
            }
        }


        var scroll = Input.mouseScrollDelta.y;
        playerObject.transform.position += -mainCamera.transform.forward * scroll * zoomSpeed;
    }
}
