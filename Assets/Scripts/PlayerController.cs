using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseCharacterController
{
    private Animator animator;
    [SerializeField]
    private GameObject bulletPrefab;

    protected override void Start()
    {
        animator = GetComponent<Animator>();
        base.Start();

        speed = 10;
        jumpForce = 10;
        gravity = 20;
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Z))
        {
            ShootBullet();
        }
    }

    private void PlayerJump()
    {
        Debug.Log("sdf");
        if (controller.isGrounded)
        {
            Debug.Log("ssfsdfdf");
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
        GameObject obj = Instantiate(bulletPrefab, transform.position + transform.forward * 1f + transform.up * 1f, Quaternion.identity);
        obj.transform.rotation = Quaternion.Euler(-90, transform.rotation.y, 0);
    }
}
