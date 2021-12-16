using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCharacterController : MonoBehaviour
{
    public Vector3 moveDir;

    public float speed;
    public float jumpForce;
    public float gravity;
    public int hp;
    protected int maxHP;
    protected float moveY;

    protected CharacterController controller;


    protected virtual void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        
    }
}
