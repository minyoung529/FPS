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
    private bool isAttacking;
    public bool playerInAttack;
    private int attackPower;
    public int score;


    private PlayerController targetPlayerController;
    private ParticleSystem particle;


    protected override void Awake()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        particle = GetComponentInChildren<ParticleSystem>();
        destinationTranform = UIManager.Instance.player.transform;
        base.Awake();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isDead) return;
            if (other.GetComponent<PlayerController>().IsDead()) return;
            if (isAttacking)
            {
                navMeshAgent.SetDestination(destinationTranform.position);
            }

            else
            {
                if(playerInAttack)
                {
                    Attack(targetPlayerController);
                }
                else
                {
                    navMeshAgent.SetDestination(other.transform.position);
                    targetPlayerController = other.GetComponent<PlayerController>();
                }
            }
        }
    }

    //액티브 true일 때 발생
    private void OnEnable()
    {
        //controller.enabled = true;
        AddHUD();
        //hud.gameObject.SetActive(false);
        isDead = false;

        SetHP();
        UpdateHP();

        Invoke("MoveToRandomPos", 2f);

        navMeshAgent.SetDestination(destinationTranform.position);
    }

    private void OnDisable()
    {
        if (hud == null) return;
        Destroy(hud.gameObject);
    }

    private void SetHP()
    {
        switch (zombieType)
        {
            case ZombieType.Basic:
                hp = 5;
                hud.maxValue = 5;
                attackPower = 1;
                score = 1;
                break;

            case ZombieType.Strong:
                hp = 10;
                hud.maxValue = 10;
                animator.speed = 0.6f;
                attackPower = 5;
                score = 5;
                break;
        }

        hud.value = hud.maxValue;
    }
    public void DestroyZombie()
    {
        TutorialEnemyGenerator.EnqueueZombie(zombieType, gameObject);
        gameObject.SetActive(false);
    }

    internal int OnHit()
    {
        hp--;
        hud.value--;

        particle.Play();

        if (hud.gameObject.activeSelf)
        {
            //hud.gameObject.SetActive(true);
        }

        if (hp <= 0)
        {
            isDead = true;
            animator.SetTrigger("Dead");
            StartCoroutine(Dead());
            return score;
        }

        return -1;
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

        animator.SetFloat("Speed", navMeshAgent.velocity.magnitude);

        UpdateHP();
    }

    private void MoveToRandomPos()
    {
        float radius = 10;
        Vector3 randomPos = Random.insideUnitSphere * radius;
        NavMeshHit hit;
        Vector3 destination;

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

    internal void Attack(PlayerController playerController)
    {
        if (isAttacking) return;
        if (playerController.IsDead()) return;

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(playerController.transform.position - transform.position), 1f);
        animator.SetTrigger("Attack");
        isAttacking = true;
        playerInAttack = true;
        navMeshAgent.isStopped = true;
        targetPlayerController = playerController;

        playerController.OnHit(attackPower,transform.position);
        Invoke("AttackEndAction", 2f);
    }

    internal void OnAttackEnd()
    {
        Invoke("AttackEndAction", 2f);
    }

    internal void ResetTarget()
    {
        navMeshAgent.isStopped = false;
        targetPlayerController = null;
        playerInAttack = false;
        isAttacking = false;

    }
    private void AttackEndAction()
    {
        isAttacking = false;
        navMeshAgent.isStopped = false;

        if (playerInAttack)
        {
            //Attack();
        }
    }
}
