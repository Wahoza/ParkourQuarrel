using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Splines.Interpolators;

public class PlayerController : MonoBehaviour
{
    [Header("RbController")]
    [SerializeField, Range(0f, 1f)] private float groundControll;
    [SerializeField, Range(0f, 1f)] private float airControll;
    [SerializeField] float moveSpeed;
    [SerializeField] float jumpHeight;
    [SerializeField] float maxUpVelocity;
    [SerializeField, Range(0f, 1f)] float maxUpAdjustmentSpeed;
    [SerializeField] FeetScript Feet;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] float jumpBuffer; //to prevent spamming of jump
    [SerializeField] float slopeLimit;
    [SerializeField] float slopeSpeedReductionFactor;
    [SerializeField] Vector3 initalScale;
    [SerializeField] Transform respawnPoint;
    float adjustedForSlope;
    float jumpTimer;
    private Rigidbody rb;

    private Vector3 moveDirection = Vector3.zero;
    private Vector2 inputVector = Vector2.zero;
    private Vector3 desiredVelocity = Vector3.zero;
    private float jumpStrength;
    private bool isGrounded = false;
    private bool lastisGrounded = true;
    private bool isSlopedGround = false;

    [Header("AnimController")]
    [SerializeField] GameObject model;
    [SerializeField] public PlayerParticles particlesController;
    private Animator animController;
    private List<Collider> ragdollColliders = new List<Collider>();
    private List<Collider> playerColliders = new List<Collider>();

    [Header("Grabbable")]
    [SerializeField] GameObject grabber;
    GrabberScript grabberScript;

    [Header("Attackable")]
    [SerializeField] GameObject attacker;
    [SerializeField] float attackCooldown;
    float stunTime;
    float attackTimer;
    PlayerAttackScript attackerScript;
    bool isStunned = false;


    //Reinitialize Stuff
    Vector3 initialPos = Vector3.zero;
    float initialMass = 0f;


    bool isInitialized = false;
    bool canMove = true;
    bool canControl = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Awake()
    {
        if(!isInitialized) {
            InitializePlayer();
        }
    }
    public void SetRespawnPoint(Transform point)
    {
        respawnPoint = point;
    }
    private void InitializePlayer()
    {
        SetColliders();

        rb = GetComponent<Rigidbody>();
        jumpStrength = Mathf.Sqrt(2 * Physics.gravity.magnitude * jumpHeight) * rb.mass;
        grabberScript = grabber.GetComponent<GrabberScript>();
        animController = model.GetComponent<Animator>();
        attackerScript = attacker.GetComponent<PlayerAttackScript>();


        
        initialPos = transform.position;
        initialMass = rb.mass;
        
        isInitialized = true;
    }
    private void SetRagdollColliders()
    {
         
        Collider[] colliders = model.gameObject.GetComponentsInChildren<Collider>();

        foreach (Collider collider in colliders) {
            collider.enabled = false;
            ragdollColliders.Add(collider);
        }
    }
    private void SetPlayerColliders()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();

        foreach (Collider collider in colliders) {
            if (!ragdollColliders.Contains(collider))
            {
                playerColliders.Add(collider);
            }
        }
    }

    private void SetColliders()
    {
        SetRagdollColliders();
        SetPlayerColliders();
    }

    public void TurnOnRagdoll()
    {
        animController.enabled = false;
        
        GetComponent<Rigidbody>().isKinematic = true;
        foreach (Collider collider in playerColliders)
        {
            collider.enabled = false;
        }
        foreach (Collider collider in ragdollColliders)
        {
            collider.enabled = true;
            collider.transform.localPosition = Vector3.zero;
            collider.attachedRigidbody.linearVelocity = Vector3.zero;
        }
        canControl = false;
    }
    public void TurnOffRagdoll()
    {
        animController.enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;

        foreach (Collider collider in playerColliders)
        {
            collider.enabled = true;
            if (collider.attachedRigidbody != null)
            {
                collider.attachedRigidbody.linearVelocity = Vector3.zero;
            }
        }

        grabberScript.ManualChangeCollider(false);
        attackerScript.ManualChangeCollider(false);
        foreach (Collider collider in ragdollColliders)
        {
            collider.enabled = false;
        }
        canControl = true;

        transform.localPosition = Vector3.zero;
    }
    public void Die()
    {

        TurnOnRagdoll();
        canMove = false;
        //enabled = false;
    }
    public void SetInputVector(Vector2 direction)
    {
        inputVector = direction;
        if ((inputVector * 100f).sqrMagnitude > 10000)
        {
            inputVector.Normalize();
        }
    }
    public void TryJump()
    {
        if (Feet.GetGrounded() && Time.time > jumpTimer && canMove && canControl && Time.timeScale != 0)
        {
            jumpTimer = Time.time + jumpBuffer;
            rb.AddForce(Vector3.up * jumpStrength, ForceMode.Impulse);


            animController.SetTrigger("jumpAction");
            particlesController.Jump(transform.position);
        }
    }
    public void ForceJump()
    {
        jumpTimer = Time.time + jumpBuffer;
        rb.AddForce(Vector3.up * jumpStrength, ForceMode.Impulse);
        animController.SetTrigger("jumpAction");
    }
    public void TryGrab()
    {
        if (!(isGrounded && canMove && canControl)) { return; }
        animController.SetTrigger("grabAction");
        grabberScript.GrabAttempt();
    }
    public void TryAttack()
    {
        if (!isGrounded)
        {
            if (grabberScript.IsHolding())
            {
                grabberScript.Throw();
            }
            return;
        }
        if (Time.time < attackTimer) { return; }

        attackTimer = Time.time + attackCooldown;

        animController.SetTrigger("attackAction");

        if (grabberScript.IsHolding())
        {
            grabberScript.Throw();
        }
        else
        {
            attackerScript.Attack();
        }
    }
    public void TryRelease()
    {
        grabberScript.Release();
    }
    
    private void MovementUpdate()
    {
        if (!canMove) {  return; }  
        if (isStunned)
        {
            if (stunTime < Time.time)
            {
                transform.rotation = Quaternion.identity;
                isStunned = false;
            }
            else
            {
                return;
            }
        }

        moveDirection.Set(inputVector.x, 0, inputVector.y);

        moveDirection *= moveSpeed * adjustedForSlope ;
        Vector3 mixedVelocity = Vector3.Lerp(rb.linearVelocity, moveDirection, (isGrounded ? groundControll : airControll));

        
        if (maxUpVelocity < rb.linearVelocity.y && !isGrounded)
        {
            float upVelocity = Mathf.Lerp(rb.linearVelocity.y, maxUpVelocity, maxUpAdjustmentSpeed);
            rb.linearVelocity = new Vector3(mixedVelocity.x, upVelocity, mixedVelocity.z);
        }
        else
        {
            rb.linearVelocity = new Vector3(mixedVelocity.x, rb.linearVelocity.y, mixedVelocity.z);
        }
        
        
    }
    private void RotateGameObjectTowardsInput()
    {
        Vector3 inputDirection = new Vector3(inputVector.x, 0f, inputVector.y);

        if (inputDirection.sqrMagnitude > 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(inputDirection, Vector3.up);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }
    public void StunPlayer(float stunDuration, Vector3 forceDirection)
    {
        isStunned = true;

        stunTime = Time.time + stunDuration;

        if (forceDirection.sqrMagnitude > 0.001f)
        {
            forceDirection.Normalize();
            Quaternion lookRotation = Quaternion.LookRotation(forceDirection, Vector3.up);
            Quaternion additionalRotation = Quaternion.Euler(90f, 0f, 0f);
            transform.rotation = lookRotation * additionalRotation;
        }
    }
    private void ModelUpdate()
    {
        if (isStunned)
        {
            return;
        }
        animController.SetBool("isGrounded", isGrounded);

        animController.SetFloat("horizontal", 0); //always 0
        animController.SetFloat("vertical", Mathf.Abs((inputVector * 100).sqrMagnitude) / 10000);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!canControl) { return; }
        isSlopedGround = true;

        isGrounded = Feet.GetGrounded();

        if (isGrounded != lastisGrounded)
        {
            if (isGrounded && transform != null) {
                particlesController.Land(transform.position);
            }

            lastisGrounded = isGrounded;
        }

        if (!isGrounded)
        {
            adjustedForSlope = 1;
        }
        RotateGameObjectTowardsInput();
        ModelUpdate();
        MovementUpdate();
    }
    public void Respawn()
    {
        TurnOffRagdoll();
        isStunned = false;

        rb.mass = initialMass;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        stunTime = 0;
        jumpStrength = Mathf.Sqrt(2 * Physics.gravity.magnitude * jumpHeight) * rb.mass;
        Feet.Respawn();

        canControl = true;

        transform.localScale = respawnPoint.localScale;
        transform.position = respawnPoint.position;
        transform.rotation = respawnPoint.rotation;

        Debug.Log("REspawned " + respawnPoint.position);
    }
    public void IncreaseJumpStrenght(float amount)
    {
        jumpStrength *= amount;
    }

    private void OnCollisionStay(Collision collision)
    {
        if(!isSlopedGround) { return; }
        foreach (ContactPoint contact in collision.contacts)
        {
            Vector3 surfaceNormal = contact.normal;

            float slopeAngle = Vector3.Angle(surfaceNormal, Vector3.up);

            if (slopeAngle > slopeLimit)
            {
                adjustedForSlope = 1 - slopeSpeedReductionFactor * (slopeAngle / 90f);
            }
            else
            {
                isSlopedGround = false;
                adjustedForSlope = 1;
                return;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 4)
        {
            particlesController.LandWater(transform.position);
        }
    }

    public void CanMove(bool v)
    {
        canMove = v;
    }
    public void CanControl(bool control)
    {
        canControl = control;   
    }
}
