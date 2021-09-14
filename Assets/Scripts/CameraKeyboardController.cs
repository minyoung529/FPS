using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraKeyboardController : MonoBehaviour
{
    public GameObject folllowingTarget;
    public Vector3 offset;
    public float distance;
    public Vector3 relativePos;
    public float verticalAngle;
    public float horizontalAngle;

    private void Start()
    {
        offset = new Vector3(0, -4, 2);
        verticalAngle = 45;
        distance = 10;
        relativePos = Quaternion.Euler(verticalAngle, horizontalAngle, 0) * new Vector3(0, 0, -distance);
    }
    
    private void LateUpdate()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            Rotate(-1);
        }

        else if (Input.GetKey(KeyCode.E))
        {
            Rotate(1f);
        }

        else
        {
            Rotate(0);
        }

        transform.position = folllowingTarget.transform.position + offset + relativePos;
        transform.LookAt(folllowingTarget.transform);
    }

    private void Rotate(float angle)
    {
        horizontalAngle += angle;
        relativePos = Quaternion.Euler(verticalAngle, horizontalAngle, 0) * new Vector3(0, 0, -distance);
    }
}
