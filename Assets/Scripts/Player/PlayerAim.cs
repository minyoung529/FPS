using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    public float turnSpeed = 15;
    private Camera mainCam;
    public Transform followTarget;
    public GameObject playerRoot;
    private Cinemachine.CinemachineVirtualCamera cam;
    public Cinemachine.AxisState xAxis;
    public Cinemachine.AxisState yAxis;

    public bool isMe;
    void Start()
    {
        mainCam = Camera.main;
        SetInitRotation();
    }
    public void StartGame(bool isMe , Cinemachine.CinemachineVirtualCamera cam)
    {
        this.isMe = isMe;
        this.cam = cam;
    }
    private void FixedUpdate()
    {
        if (isMe)
        {
            xAxis.Update(Time.fixedDeltaTime);
            yAxis.Update(Time.fixedDeltaTime);
        }
        // Follow Target Rotate for Cam Rotate

        Rotate();
    }

    private void Rotate()
    {
        followTarget.eulerAngles = new Vector3(yAxis.Value, xAxis.Value, 0);

        // Player Rotate
        playerRoot.transform.rotation = Quaternion.Slerp(playerRoot.transform.rotation, Quaternion.Euler(0, xAxis.Value, 0), turnSpeed * Time.fixedDeltaTime);
    }

    public void NetPlayerRotation(float x, float y)
    {
        xAxis.Value = x;
        yAxis.Value = y;
    }

    public void SetInitRotation()
    {
        Vector3 dir = Vector3.zero - transform.position;
        Quaternion lookRot = Quaternion.LookRotation(dir, Vector3.up);
        xAxis.Value = lookRot.eulerAngles.y;
    }
}
