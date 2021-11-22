using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastAim : MonoBehaviour
{
    public Transform cameraAimTarget;
    public WeaponController[] weapons;
    Vector2 screenCenterPos;
    private LayerMask mouseColliderLayerMask;

    private Weapon currentWeapon;

    Ray ray;
    RaycastHit raycastInfo;

    private Camera mainCam;

    [SerializeField] WeaponController wc;

    public Transform aimPosition;

    private bool started = false;

    public void StartGame()
    {
        mainCam = Camera.main;
        currentWeapon = Weapon.Handgun;
        screenCenterPos = new Vector2(Screen.width / 2f, Screen.height / 2f);

        cameraAimTarget = mainCam.transform.GetChild(0);
        started = true;
    }

    void Update()
    {
        if (!started) return;

        ray.origin = mainCam.transform.position + mainCam.transform.forward*6f;
        ray.direction = cameraAimTarget.position - ray.origin;

        if (Physics.Raycast(ray, out raycastInfo, 999f, mouseColliderLayerMask))
        {
            aimPosition.position = raycastInfo.point;
            wc.aimPosition = raycastInfo.point;
        }

        if (Input.GetKeyDown(KeyCode.R) && wc.currentAmmo != wc.maxAmmo)
        {
            wc.Reloading();
        }

        if(Input.GetKeyUp(KeyCode.Alpha1))
        {
            SwapWeapon(Weapon.Handgun);
        }

        else if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            SwapWeapon(Weapon.Rifle);
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && !wc.isReloading)
        {
            wc.ShootBullet();
        }
    }

    private void SwapWeapon(Weapon weaponType)
    {
        if(weaponType == currentWeapon)
        {
            return;
        }

        wc = weapons[(int)weaponType];

        weapons[(int)weaponType].gameObject.SetActive(true);
        weapons[(int)currentWeapon].gameObject.SetActive(false);

        //switch (weapon)
        //{
        //    case Weapon.Handgun:
        //        break;
        //    case Weapon.Rifle:
        //        break;
        //}

        currentWeapon = weaponType;

        UIManager.Instance.ChangeCurrentAmmoText(wc.currentAmmo, wc.maxAmmo);
    }
}
