using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastAim : MonoBehaviour
{
    [SerializeField] private int currentAmmo;
    [SerializeField] private int maxAmmo;
    private Vector3 aimPosition;
    public Transform bulletSpawn;
    public Transform cameraAimTarget;

    Vector2 screenCenterPos;
    private LayerMask mouseColliderLayerMask;
    private bool isReloading;
    [SerializeField]
    private GameObject bulletPrefab;
    private ParticleSystem gunEffect;
    Animator animator;

    Ray ray;
    RaycastHit raycastInfo;

    private Camera mainCam;

    private void Start()
    {
        gunEffect = transform.GetChild(2).GetComponent<ParticleSystem>();
        animator = GetComponent<Animator>();
        maxAmmo = 10;
        currentAmmo = maxAmmo;
        mainCam = Camera.main;

        screenCenterPos = new Vector2(Screen.width / 2f, Screen.height / 2f);
    }

    void Update()
    {
        //Ray ray = mainCam.ScreenPointToRay(screenCenterPos);

        ray.origin = mainCam.transform.position + mainCam.transform.forward*6f;
        ray.direction = cameraAimTarget.position - ray.origin;

        if (Physics.Raycast(ray, out raycastInfo, 999f, mouseColliderLayerMask))
        {
            aimPosition = raycastInfo.point;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && !isReloading)
        {
            ShootBullet();
        }

        if (Input.GetKeyDown(KeyCode.R) && currentAmmo != maxAmmo)
        {
            Invoke("Reload", 3f);
        }
    }

    private void Reload()
    {
        isReloading = false;
        currentAmmo = maxAmmo;
        UIManager.Instance.ChangeCurrentAmmoText(currentAmmo);

    }

    private void ShootBullet()
    {
        currentAmmo--;

        if (currentAmmo == 0)
        {
            isReloading = true;
            Invoke("Reload", 3f);
        }

        gunEffect.Play();
        gunEffect.transform.rotation = transform.rotation;
        UIManager.Instance.ChangeCurrentAmmoText(currentAmmo);
        animator.SetTrigger("Shoot");

        Vector3 aimDir = (aimPosition - bulletSpawn.position).normalized;

        Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.LookRotation(aimDir));
    }
}
