using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerConfig playerConfig;
    private PlayerController mover;
    private GrabbablePlayer grabber;
    private SpeechScript speechScript;
    private PlayerInputMap controls;
    private PauseScript pauseScript;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Awake()
    {
        mover = GetComponent<PlayerController>();
        grabber = GetComponent<GrabbablePlayer>();
        controls = new PlayerInputMap();

        var pauseFind = GameObject.FindGameObjectWithTag("PauseMenu");

        if(pauseFind != null)
        {
            pauseScript = pauseFind.GetComponent<PauseScript>();
        }
    }

    // Update is called once per frame
    public void OnMove(CallbackContext context)
    {
        if (context.ReadValue<Vector2>() == null) { return; }
        if (mover == null) { return; }
        mover.SetInputVector(context.ReadValue<Vector2>());
    }
    public void SetSpeechScript(SpeechScript script)
    {
        speechScript = script;
    }
    public void InitializePlayer(PlayerConfig pc)
    {
        
        playerConfig = pc;
        playerConfig.Input.onActionTriggered += Input_onActionTriggered;  
    }

    public void Input_onActionTriggered(CallbackContext obj)
    {
        
        if(obj.action.name == controls.Player.Move.name)
        {
            OnMove(obj);
        }
        if(obj.action.name == controls.Player.Interact.name)
        {
            OnGrab(obj);
        }
        if (obj.action.name == controls.Player.Attack.name)
        {
            OnAttack(obj);
        }
        if (obj.action.name == controls.Player.Jump.name)
        {
            OnJump(obj);
        }
        if (obj.action.name == controls.Player.Talk.name)
        {
            OnTalk(obj);
        }
        if (obj.action.name == controls.Player.Pause.name) {
            OnPause();
        }
    }
    public void OnTalk(CallbackContext context)
    {
        if (speechScript == null) { return; }
        if (context.phase == InputActionPhase.Started)
        {
            speechScript.StartSpeak(context.ReadValue<Vector2>());
        }
    }
    public void OnJump(CallbackContext context)
    {
        if (mover == null) { return; }
        if (context.phase == InputActionPhase.Started)
        {
            mover.TryJump();
            grabber.JumpInput();
        }
    }
    public void OnAttack(CallbackContext context)
    {
        if (mover == null) { return; }
        if (context.phase == InputActionPhase.Started)
        {
            mover.TryAttack();
        }
    }
    public void OnGrab(CallbackContext context)
    {
        if (mover == null) { return; };
        if (context.phase == InputActionPhase.Started)
        {
            mover.TryGrab();
        }
        if (context.phase == InputActionPhase.Canceled)
        {
            mover.TryRelease();
        }
    }
    public void OnPause()
    {
        if (pauseScript != null)
        {
            pauseScript.Pause(playerConfig);
        }
        else
        {
            Debug.Log("Pause not found");
        }
    }
}
