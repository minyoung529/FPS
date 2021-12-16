using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRecoil : MonoBehaviour
{
    public PlayerAim PlayerAim;
    public float verticalRecoil = 1f;
    public float horizontalRecail = 1f;
    public float recoilConstant = 1f;
    public Cinemachine.CinemachineImpulseSource impulse;

    public void GenerateRecoil()
    {
        //if (PlayerAim.IsAiming())
        //{
            //return;
        //}
        //else
        //{
            //recoilConstant = 1f;
        //}

        float randomX = Random.Range(-horizontalRecail, horizontalRecail);

        impulse.GenerateImpulse(Camera.main.transform.forward);
        //PlayerAim.yAxis.Value -= verticalRecoil * recoilConstant;
        //PlayerAim.xAxis.Value += randomX * recoilConstant;
    }

    internal void SetRecoilConstance(float c)
    {
        recoilConstant = c;
    }
}
