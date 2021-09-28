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
        transform.Rotate(90, 0, 0);

        originPos = transform.position;
        rigid.AddForce(transform.up * speed);
    }

    private void OnTriggerEnter(Collider other)
    {


        //if (other.gameObject.CompareTag("Enemy"))
        //{
        //    Destroy(other.gameObject);
        //    Destroy(gameObject);
        //}

        //Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Enemy":
                collision.gameObject.GetComponent<ZombieController>()?.OnHit();
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