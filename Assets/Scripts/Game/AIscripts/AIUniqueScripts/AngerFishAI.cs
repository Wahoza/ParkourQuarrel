using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AngerFishAI : IAIContollerScript
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] Animator fishBodyAnimator;
    [SerializeField] Rigidbody fishRB;
    [SerializeField] float searchRange;
    Transform goalPoint;

    [Header("Movement Stuff")]
    [SerializeField] float rotationSpeed;
    [SerializeField] float moveSpeed;
    [SerializeField] float attackSpeedMultiplier;
    [SerializeField] float selfBumpForce;

    [Header("Repositioning Stuff")]
    [SerializeField] Vector3Int MaxLookValues;
    [SerializeField] Vector3Int MinLookValues;
    private Vector3 moveDirection;

    [Header("IdleStuff")]
    [SerializeField] float bumpDuration;
    private float bumpTimer;
    private bool isBumped;
    
    private Transform SelectedTarget;
    private int WaterLayer = 4;
    enum fishState
    {
        idle = 0,
        attacking = 1,
        outOfWater = 2
    }
    fishState currentState = fishState.outOfWater;
    void Start()
    {
        System.Random r = new System.Random();
        moveDirection = new Vector3(r.Next(MinLookValues.x, MaxLookValues.x), r.Next(MinLookValues.y, MaxLookValues.y), r.Next(MinLookValues.z, MaxLookValues.z)).normalized;
    }
    void SwitchState(fishState nextState)
    {
        currentState = nextState;
        switch (nextState)
        {
            case fishState.idle:
                InitializeIdle();
                return;
            case fishState.attacking:
                InitializeAttack();
                return;
            case fishState.outOfWater:
                InitializeOutOfWater();
                return;
        }
    }
    void InitializeOutOfWater() {
        fishBodyAnimator.SetBool("isOutOfWater", true);
        fishBodyAnimator.SetBool("isAttacking", false);
    }
    void OutOfWaterUpdate()
    {
        Debug.Log("OutOfWater");
    }
    void InitializeIdle() {
        fishBodyAnimator.SetBool("isOutOfWater", false);
        fishBodyAnimator.SetBool("isAttacking", false);

        System.Random r = new System.Random();
        moveDirection = new Vector3(r.Next(MinLookValues.x, MaxLookValues.x), r.Next(MinLookValues.y, MaxLookValues.y), r.Next(MinLookValues.z, MaxLookValues.z)).normalized;
        fishRB.linearVelocity = Vector3.zero;
        fishRB.useGravity = false;
    }

    void IdleUpdate()
    {
        if (!isBumped)
        {
            transform.LookAt(moveDirection + transform.position);
            fishRB.linearVelocity = Vector3.Lerp(fishRB.linearVelocity, transform.forward, moveSpeed);
        }
        else
        {
            if (bumpTimer < Time.deltaTime)
            {
                isBumped = false;
            }
        }
        Debug.Log(moveDirection);
    }
    void InitializeAttack() {
        fishBodyAnimator.SetBool("isOutOfWater", false);
        fishBodyAnimator.SetBool("isAttacking", false);
    }
    void AttackUpdate()
    {
        Debug.Log("Attacking");
    }

    // Update is called once per frame
    // rotate away when collision on face
    // Sphere Collider is like a nose
    void Update()
    {
        Debug.Log(WaterLayer);
        Debug.Log(currentState);
        switch (currentState)
        {
            case fishState.idle:
                IdleUpdate(); return;
            case fishState.attacking:
                AttackUpdate(); return;
            case fishState.outOfWater:
                OutOfWaterUpdate(); return;
        }

    }
    private void OnTriggerStay(Collider other)
    {
        if(currentState == fishState.outOfWater && other.gameObject.layer == WaterLayer)
        {
            SwitchState(fishState.idle);

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(currentState != fishState.outOfWater && other.gameObject.layer == WaterLayer)
        {
            SwitchState(fishState.outOfWater);
            fishRB.useGravity = true;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(currentState == fishState.idle)
        {
            //bump backwards from collision
            isBumped = true;
            bumpTimer = Time.time;
            fishRB.linearVelocity = Vector3.zero;
            fishRB.AddForce(-moveDirection * selfBumpForce, ForceMode.Impulse);
            //switch direction to a new direction
            System.Random r = new System.Random();
            moveDirection = new Vector3(r.Next(MinLookValues.x, MaxLookValues.x), r.Next(MinLookValues.y, MaxLookValues.y), r.Next(MinLookValues.z, MaxLookValues.z)).normalized;
        }
    }
}
