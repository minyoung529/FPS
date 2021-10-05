using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject folllowingTarget;
    public Vector3 offset;
    public float distance;
    public Vector3 relativePos;
    public float verticalAngle;
    public float horizontalAngle;
    [SerializeField]
    private Vector3 lookOffset;

    [SerializeField] float rotateSensitivity;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        lookOffset = new Vector3(1f, 1.5f, 0f);
        offset = new Vector3(0, -4, 2);
        verticalAngle = 45;
        distance = 10;
        rotateSensitivity = 5f;
        relativePos = Quaternion.Euler(verticalAngle, horizontalAngle, 0) * new Vector3(0, 0, -distance);
    }

    private void LateUpdate()
    {
        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = -Input.GetAxisRaw("Mouse Y");
        Rotate(mouseX * rotateSensitivity, mouseY * rotateSensitivity);

        transform.position = folllowingTarget.transform.position + offset + relativePos;
        transform.LookAt(folllowingTarget.transform.position + transform.TransformDirection(lookOffset));
    }

    private void Rotate(float x, float y)
    {
        horizontalAngle += x;
        verticalAngle += y;
        relativePos = Quaternion.Euler(Mathf.Clamp(verticalAngle, -45, 80), horizontalAngle, 0) * new Vector3(0, 0, -distance);
    }
}
