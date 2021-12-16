using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : BaseCharacterController
{
    private Animator animator;

    private bool isDead;
    [SerializeField]
    private ParticleSystem hitEffect;
    private bool isMe;
    private bool started;

    PlayerSkinManager skinManager;

    public RaycastAim raycastAim;
    public Cinemachine.CinemachineVirtualCamera cinemachineCam;

    private WaitForSeconds transformDelay = new WaitForSeconds(0.1f);

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

    public void StartGame(bool isMe)
    {
        this.isMe = isMe;
        started = true;
        raycastAim.StartGame(isMe);
        cinemachineCam.gameObject.SetActive(true);

        if (isMe)
        {
            cinemachineCam.enabled = true;
            StartCoroutine(SendTranformCoroutine());
        }
    }

    private void FixedUpdate()
    {
        if (!started) return;
        if (UIManager.Instance.GetIsPaused()) return;
        if (isDead) return;


        if(isMe)
        {
            moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            UpdateMoveDirection(transform.TransformDirection(moveDir));
        }
        CharacterMove();
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

    private IEnumerator SendTranformCoroutine()
    {
        while (!isDead)
        {
            //Custom Structure
            //string[]
            //NetTransform trans =
            //    new NetTransform(transform.position.x, transform.position.y, transform.position.z,
            //    transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

            string[] trans = new string[2];
            trans[0] = string.Format("{0}:{1}:{2}", transform.position.x, transform.position.y, transform.position.z);
            trans[1] = string.Format("{0}:{1}:{2}", transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
            NetClient.Instance.SendTranform(trans);
            yield return transformDelay;
        }
    }

    private void CharacterMove()
    {
        Rotate();
        moveDir *= speed;

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

    public void UpdateMoveDirection(Vector3 dir)
    {
        moveDir = dir.normalized * speed;
    }
}

[Serializable]
public struct NetTransform
{
    Position position;
    Rotation rotation;

    [Serializable]
    public struct Position
    {
        public float posX;
        public float posY;
        public float posZ;
    }

    [Serializable]
    public struct Rotation
    {
        public float rotX;
        public float rotY;
        public float rotZ;
    }

    public NetTransform(float x, float y, float z, float rX, float rY, float rZ)
    {
        position.posX = x;
        position.posY = y;
        position.posZ = z;

        rotation.rotX = rX;
        rotation.rotY = rY;
        rotation.rotZ = rZ;
    }
}