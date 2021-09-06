using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseCharacterController
{
    protected override void Start()
    {
        base.Start();

        speed = 5;
        jumpForce = 3;
        gravity = 9.8f;
    }

    void Update()
    {
        moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), moveDir.y, Input.GetAxisRaw("Vertical"));

        if (Input.GetButton("Jump"))
        {
            PlayerJump();
        }

        moveDir.y -= gravity * Time.deltaTime;
        controller.Move(moveDir * speed * Time.deltaTime);
    }

    private void PlayerJump()
    {
        if(controller.isGrounded)
            moveDir.y = jumpForce;
    }
}
