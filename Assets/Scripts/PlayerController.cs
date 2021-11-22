using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseCharacterController
{
    private Animator animator;

    private bool isDead;
    [SerializeField]
    private ParticleSystem hitEffect;
    private bool started;

    PlayerSkinManager skinManager;

    public RaycastAim raycastAim;
    public Cinemachine.CinemachineVirtualCamera cinemachineCam;

    protected override void Awake()
    {
        animator = GetComponent<Animator>();
        skinManager = GetComponent<PlayerSkinManager>();
        base.Awake();

        speed = 10;
        jumpForce = 20;
        gravity = 50;


        SetHP();

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
        transform.rotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, Camera.main.transform.eulerAngles.z);

    }

    public void StartGame()
    {
        started = true;
        raycastAim.StartGame();
        cinemachineCam.gameObject.SetActive(true);
        cinemachineCam.enabled = true;
    }

    private void FixedUpdate()
    {
        if (!started) return;
        if (UIManager.Instance.GetIsPaused()) return;
        if (isDead) return;
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


    private void SetHP()
    {
        maxHP = 20;
        hp = maxHP;
    }

    public void OnHit(int attackPower, Vector3 zombiePos)
    {
        hp -= attackPower;
        UIManager.Instance.UpdatePlayerHP(maxHP, hp);
        //Vector3 effectPos = (zombiePos - transform.position) / 2;
        //effectPos.y = 1;

        //hitEffect.transform.position = effectPos;
        hitEffect.Play();
        if (hp < 0)
        {
            OnDead();
            UIManager.Instance.UpdatePlayerHP(maxHP, hp);
        }

        skinManager.StartInvincible();
        Invoke("StopInvincible", 3f);
    }

    private void StopInvincible()
    {
        skinManager.StopInvincible();
    }

    public bool IsDead()
    {
        return isDead;
    }

    private void OnDead()
    {
        hp = 0;
        isDead = true;
        controller.enabled = false;
        animator.SetBool("Dead", true);
        UIManager.Instance.GameOver();
    }
}
