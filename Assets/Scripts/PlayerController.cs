using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseCharacterController
{
    private Animator animator;
    [SerializeField]
    private GameObject bulletPrefab;

    [SerializeField] private int currentAmmo;
    [SerializeField] private int maxAmmo;
    private bool isReloading;
    Vector2 screenCenterPos;
    [SerializeField]
    private LayerMask mouseColliderLayerMask;
    private Vector3 aimPosition;
    public Transform bulletSpawn;

    protected override void Awake()
    {
        animator = GetComponent<Animator>();
        base.Awake();

        speed = 10;
        jumpForce = 20;
        gravity = 50;

        maxAmmo = 10;
        currentAmmo = maxAmmo;

        screenCenterPos = new Vector2(Screen.width / 2f, Screen.height / 2f);
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPos);
        if(Physics.Raycast(ray,out RaycastHit hit, 999f, mouseColliderLayerMask))
        {
            aimPosition = hit.point;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && !isReloading)
        {
            ShootBullet();
        }

        if(Input.GetKeyDown(KeyCode.R) && currentAmmo != maxAmmo)
        {
            Invoke("Reload", 3f);
        }
    }

    private void PlayerJump()
    {
        if (controller.isGrounded)
        {
            moveY = jumpForce;
        }
    }

    private void Rotate()
    {
        transform.rotation =
            Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, Camera.main.transform.eulerAngles.z);
    }

    private void FixedUpdate()
    {
        Rotate();
        moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        moveDir *= speed;
        moveDir = transform.TransformDirection(moveDir);

        if (Input.GetButton("Jump"))
        {
            PlayerJump();
        }

        if (!controller.isGrounded)
        {
            moveY -= gravity * Time.deltaTime;
        }

        else if (controller.isGrounded)
        {
            //moveY = 0;
        }

        moveDir.y = moveY;
        controller.Move(moveDir * Time.deltaTime);
        animator.SetFloat("speed", controller.velocity.magnitude);
    }

    private void ShootBullet()
    {
        currentAmmo--;

        if (currentAmmo == 0)
        {
            isReloading = true;
            Invoke("Reload", 3f);
        }
        UIManager.Instance.ChangeCurrentAmmoText(currentAmmo);
        animator.SetTrigger("Shoot");

        Vector3 aimDir = (aimPosition - bulletSpawn.position).normalized;

        Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.LookRotation(aimDir,Vector3.up));
    }

    private void Reload()
    {
        isReloading = false;
        currentAmmo = maxAmmo;
        UIManager.Instance.ChangeCurrentAmmoText(currentAmmo);

    }
}
