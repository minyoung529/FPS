using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZombieType = TutorialEnemyGenerator.ZombieType;

public class ZombieController : BaseCharacterController
{
    private Animator animator;

    public ZombieType zombieType;
    private Slider hud;


    protected override void Awake()
    {
        animator = GetComponent<Animator>();
        base.Awake();
    }

    //액티브 true일 때 발생
    private void OnEnable()
    {
        controller.enabled = true;
        AddHUD();

        SetHP();
        UpdateHP();
    }

    private void OnDisable()
    {
        Destroy(hud?.gameObject);
    }

    private void SetHP()
    {
        switch (zombieType)
        {
            case ZombieType.Basic:
                hp = 5;
                hud.maxValue = 5;
                break;
            case ZombieType.Strong:
                hp = 10;
                hud.maxValue = 10;
                break;
        }

        hud.value = hud.maxValue;
    }
    public void DestroyZombie()
    {
        TutorialEnemyGenerator.EnqueueZombie(zombieType, gameObject);
        gameObject.SetActive(false);
    }

    internal void OnHit()
    {
        hp--;
        hud.value--;

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
        hud = UIManager.Instance.AddEnemyHUD().GetComponent<Slider>();
    }

    private void UpdateHP()
    {
        if(hud != null)
        {
            UIManager.Instance.UpdateHUDPosition(hud.gameObject, transform.position + transform.up * 2);
        }
    }

    private void Update()
    {
        UpdateHP();
    }
}
