using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseCharacterController
{
    protected override void Start()
    {
        base.Start();

        speed = 10;
        jumpForce = 10;
        gravity = 20f;
    }

    void Update()
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

        moveDir.y = moveY;
        controller.Move(moveDir * Time.deltaTime);
    }

    private void PlayerJump()
    {
        if (controller.isGrounded)
            moveY = jumpForce;
    }

    private void Rotate()
    {
        transform.rotation =
            Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, Camera.main.transform.eulerAngles.z);
    }
}
