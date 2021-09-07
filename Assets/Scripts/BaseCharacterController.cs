using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCharacterController : MonoBehaviour
{
    public Vector3 moveDir;

    public float speed;
    public float jumpForce;
    public float gravity;
    protected float moveY;

    protected CharacterController controller;


    protected virtual void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        
    }
}
