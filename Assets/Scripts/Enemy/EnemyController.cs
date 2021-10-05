using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : BaseCharacterController
{
    public GameObject enem;

    protected override void Awake()
    {
        base.Awake();

        speed = 1;
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
        if (controller.isGrounded)
            moveDir.y = jumpForce;
    }

}
