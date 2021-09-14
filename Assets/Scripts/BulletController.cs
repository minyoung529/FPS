using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    private float speed = 1500;
    private Rigidbody rigid = null;
    private float maxDistance = 400;
    private Vector3 originPos;


    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        originPos = transform.position;
        rigid.AddForce(transform.up * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }

        //Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            //Destroy(collision.gameObject);
            collision.gameObject.GetComponent<ZombieController>()?.OnHit();
            Destroy(gameObject);
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