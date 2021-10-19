using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    public float turnSpeed = 15;
    private Camera mainCam;
    public Transform followTarget;
    public GameObject playerRoot;


    public Cinemachine.AxisState xAxis;
    public Cinemachine.AxisState yAxis;

    void Start()
    {
        mainCam = Camera.main;

    }

    private void FixedUpdate()
    {

        // Follow Target Rotate for Cam Rotate
        xAxis.Update(Time.fixedDeltaTime);
        yAxis.Update(Time.fixedDeltaTime);

        followTarget.eulerAngles = new Vector3(yAxis.Value, xAxis.Value, 0);

        // Player Rotate
        float camY = mainCam.transform.rotation.eulerAngles.y;
        playerRoot.transform.rotation = Quaternion.Slerp(playerRoot.transform.rotation, Quaternion.Euler(0, camY, 0), turnSpeed * Time.fixedDeltaTime);
    }

}
