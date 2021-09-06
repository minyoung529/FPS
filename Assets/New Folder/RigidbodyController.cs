using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyController : MonoBehaviour
{
    private Rigidbody rigid;
    [SerializeField]
    private float power = 1.2f;
    private float jumpForce = 17f;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector3 dir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        float xMove = Input.GetAxisRaw("Horizontal");
        float zMove = Input.GetAxisRaw("Vertical");

        if (Input.GetButton("Jump"))
        {
            rigid.AddForce(Vector3.up * jumpForce);
        }

        rigid.AddForce((Vector3.right * xMove + Vector3.up * zMove) * power);
    }
}
