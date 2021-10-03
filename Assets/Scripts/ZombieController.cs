using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using ZombieType = TutorialEnemyGenerator.ZombieType;

public class ZombieController : BaseCharacterController
{
    private Animator animator;

    public ZombieType zombieType;
    private Slider hud;

    private NavMeshAgent navMeshAgent;
    public Transform destinationTranform;

    private bool isDead;

    private Rigidbody rigid;

    protected override void Awake()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        rigid = GetComponent<Rigidbody>();
        destinationTranform = UIManager.Instance.player.transform;
        base.Awake();
    }

    private void OnTriggerStay(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            if (isDead) return;

            navMeshAgent.SetDestination(destinationTranform.position);
        }
    }

    //액티브 true일 때 발생
    private void OnEnable()
    {
        //controller.enabled = true;
        AddHUD();
        isDead = false;

        SetHP();
        UpdateHP();

        Invoke("MoveToRandomPos", 2f);

        navMeshAgent.SetDestination(destinationTranform.position);
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
            isDead = true;
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
        if (hud != null)
        {
            UIManager.Instance.UpdateHUDPosition(hud.gameObject, transform.position + transform.up * 2);
        }
    }

    private void Update()
    {
        if (isDead) return;
        if (navMeshAgent.remainingDistance <= 0)
        {
            float time = Random.Range(3f, 4f);
            Invoke("MoveToRandomPos", time);
        }

        animator.SetFloat("Speed", navMeshAgent.speed);

        UpdateHP();
    }

    private void MoveToRandomPos()
    {
        float radius = 10;
        Vector3 randomPos = Random.insideUnitSphere * radius;
        NavMeshHit hit;
        Vector3 destination = Vector3.zero;
        if (NavMesh.SamplePosition(randomPos, out hit, radius, 1))
        {
            destination = hit.position;
        }

        else
        {
            destination = transform.position - randomPos;
        }

        navMeshAgent.SetDestination(destination);
    }
}
