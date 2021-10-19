using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    public float turnSpeed = 15;
    private Camera mainCam;

    public Cinemachine.AxisState xAxis;
    public Cinemachine.AxisState yAxis;

    public Transform followTarget;

    void Start()
    {
        mainCam = Camera.main;
    }

    private void FixedUpdate()
    {
        
        xAxis.Update(Time.fixedDeltaTime);
        yAxis.Update(Time.fixedDeltaTime);

        followTarget.eulerAngles = new Vector3(yAxis.Value, xAxis.Value, 0);

        float camY = mainCam.transform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, camY, 0), turnSpeed * Time.deltaTime);
    }
}
