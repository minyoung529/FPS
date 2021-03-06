using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    private float speed = 1500;
    private Rigidbody rigid = null;
    private float maxDistance = 400;
    private Vector3 originPos;
    int damage;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        transform.Rotate(90, 0, 0);

        originPos = transform.position;
        rigid.AddForce(transform.up * speed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Enemy":
                ZombieController ec = collision.gameObject.GetComponent<ZombieController>();
                if (ec == null) return;
                int score = ec.OnHit(damage);
                if (score > 0)
                {
                    UIManager.Instance.ChangeScore(score);
                }
                Destroy(gameObject);
                break;
        }
    }

    private void Update()
    {
        if (Vector3.Distance(originPos, transform.position) > maxDistance)
        {
            Destroy(gameObject);
        }
    }
}