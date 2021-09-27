using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZombieType = TutorialEnemyGenerator.ZombieType;

public class ZombieController : BaseCharacterController
{
    private Animator animator;

    public ZombieType zombieType;
    private GameObject hud;


    protected override void Awake()
    {
        animator = GetComponent<Animator>();
        base.Awake();
        SetHP();
    }

    //��Ƽ�� true�� �� �߻�
    private void OnEnable()
    {
        controller.enabled = true;
        SetHP();
        AddHUD();
        UpdateHP();
    }

    private void SetHP()
    {
        switch (zombieType)
        {
            case ZombieType.Basic:
                hp = 5;
                break;
            case ZombieType.Strong:
                hp = 10;
                break;
        }
    }
    public void DestroyZombie()
    {
        TutorialEnemyGenerator.EnqueueZombie(zombieType, gameObject);
        gameObject.SetActive(false);
    }

    internal void OnHit()
    {
        hp--;

        if (hp > 0)
        {
            animator.SetTrigger("Attack");
        }

        else if (hp <= 0)
        {
            animator.SetTrigger("Dead");
            StartCoroutine(Dead());
        }
    }

    IEnumerator Dead()
    {
        yield return new WaitForSeconds(2f);
        DestroyZombie();
    }

    private void AddHUD()
    {
        hud = UIManager.Instance.AddEnemyHUD();
    }

    private void UpdateHP()
    {
        UIManager.Instance.UpdateHUDPosition(hud, transform.position/* + transform.up * 2*/);
    }

    private void Update()
    {
        UpdateHP();
    }
}