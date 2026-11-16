using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class HandleHandInputs : MonoBehaviour
{
    private PlayerInputMap controls;
    [SerializeField] private HandController controller;
    private void Awake()
    {
        controls = new PlayerInputMap();
    }

    // Update is called once per frame
    public void OnMove(CallbackContext context)
    {
        if (context.ReadValue<Vector2>() == null) { return; }
        if (controller == null) { return; }
        controller.SetInputVector(context.ReadValue<Vector2>());
    }
    public void InitializePlayer(PlayerConfig pc, RectTransform selectableObjectsHolder)
    {
        controller.Initialize(pc, selectableObjectsHolder);
        pc.Input.onActionTriggered += Input_onUITriggered;
    }

    private void Input_onUITriggered(CallbackContext obj)
    {
        if (obj.action.name == controls.Player.Move.name)
        {
            OnMove(obj);
        }
        if (obj.action.name == controls.Player.Decline.name)
        {
            OnBPress(obj);
        }
        if (obj.action.name == controls.Player.Confirm.name)
        {
            OnAPress(obj);
        }
        if (obj.action.name == controls.Player.SwapLeft.name)
        {

            OnSwap(obj, -1);
        }
        if (obj.action.name == controls.Player.SwapRight.name)
        {

            OnSwap(obj, 1);
        }
    }

    public void OnBPress(CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            controller.SwitchState(-1);
        }
    }
    public void OnAPress(CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            controller.SwitchState(1);
        }
    }
    public void OnSwap(CallbackContext context, int swapDir)
    {
        if (context.phase == InputActionPhase.Started)
        {
            controller.SwapHeld(swapDir);
        }
    }

}
