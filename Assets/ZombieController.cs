using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZombieType = TutorialEnemyGenerator.ZombieType;

public class ZombieController : BaseCharacterController
{
    private Animator animator;

    public ZombieType zombieType;

    protected override void Awake()
    {
        animator = GetComponent<Animator>();
        base.Awake();
        SetHP();
    }

    //액티브 true일 때 발생
    private void OnEnable()
    {
        controller.enabled = true;
        SetHP();

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
}
