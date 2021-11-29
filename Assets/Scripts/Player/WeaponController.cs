using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Weapon
{
    Handgun,
    Rifle
}
public class WeaponController : MonoBehaviour
{
    public Weapon weaponType;
    public bool isReloading = false;
    public int currentAmmo;
    public int maxAmmo;
    [SerializeField] private ParticleSystem gunEffect;
    [SerializeField] private Animator animator;
    public Vector3 aimPosition;
    public Transform bulletSpawn;
    [SerializeField] private GameObject bulletPrefab;
    int damage;
    float bulletSpeed;

    [SerializeField] private GameObject handGun;
    [SerializeField] private GameObject rifle;

    public Cinemachine.CinemachineImpulseSource impulse;

    private void Start()
    {
        currentAmmo = maxAmmo;

        switch (weaponType)
        {
            case Weapon.Handgun:
                damage = 2;
                bulletSpeed = 5000;
                maxAmmo = 10;
                break;

            case Weapon.Rifle:
                damage = 1;
                bulletSpeed = 5000;
                maxAmmo = 50;
                break;
        }

    }

    private void Reload()
    {
        isReloading = false;
        currentAmmo = maxAmmo;
        UIManager.Instance.ChangeCurrentAmmoText(currentAmmo,maxAmmo);
    }

    public void ShootBullet()
    {
        currentAmmo--;

        if (currentAmmo == 0)
        {
            isReloading = true;
            Invoke("Reload", 3f);
        }

        impulse.GenerateImpulse();
        gunEffect.Play();
        gunEffect.transform.rotation = transform.rotation;
        UIManager.Instance.ChangeCurrentAmmoText(currentAmmo,maxAmmo);
        animator.SetTrigger("Shoot");

        Vector3 aimDir = (aimPosition - bulletSpawn.position).normalized;

        Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.LookRotation(aimDir));
    }

    public void Reloading()
    {
        Invoke("Reload", 3f);
    }

    public void ChangeWeapon()
    {
        if (weaponType == Weapon.Rifle)
        {
            weaponType = Weapon.Handgun;
            handGun.SetActive(true);
            rifle.SetActive(false);
        }
        else
        {
            weaponType = Weapon.Rifle;
            handGun.SetActive(false);
            rifle.SetActive(true);
        }

        switch (weaponType)
        {
            case Weapon.Handgun:
                damage = 2;
                bulletSpeed = 5000;
                maxAmmo = 10;
                break;

            case Weapon.Rifle:
                damage = 1;
                bulletSpeed = 5000;
                maxAmmo = 50;
                break;
        }

        UIManager.Instance.ChangeCurrentAmmoText(currentAmmo, maxAmmo);
    }
}
