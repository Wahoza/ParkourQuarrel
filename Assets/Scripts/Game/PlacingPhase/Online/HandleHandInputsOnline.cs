
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

using static UnityEngine.InputSystem.InputAction;

public class HandleHandInputsOnline : NetworkBehaviour
{
    private PlayerInputMap controls;
    [SerializeField] private HandControllerOnline controller;
    [SerializeField] private PlayerInput input;

    public int id {  get; private set; }


    // Update is called once per frame
    public void OnMove(CallbackContext context)
    {
        if (context.ReadValue<Vector2>() == null) { return; }
        if (controller == null) { return; }
        controller.SetInputVector(context.ReadValue<Vector2>());
    }

    [ServerRpc(RequireOwnership = false)]
    public void InitializePlayerHandServerRpc(int ColorId, int handId)
    {
        InitializePlayerHandClientRpc(ColorId, handId);
    }

    [ClientRpc]
    private void InitializePlayerHandClientRpc(int ColorId, int handId)
    {
        Color color = OnlineGameFlowManager.Instance.GetPlayerColor(ColorId);
        bool isLocalPlayer = (handId == PlayerInteractionManager.Instance.GetPlayerId());



        var selectableObjectsHolder = PlayerInteractionManager.Instance.GetSelectableObjectsHolder();

        controller.Initialize(color, selectableObjectsHolder, isLocalPlayer, handId);

        Debug.Log(isLocalPlayer);
        Debug.Log("hand Id " + handId);
        id = handId;

        ServerGameManager.Instance.RegisterHand(controller);

        if (!isLocalPlayer)
        {
            input.enabled = false;
            Debug.Log("Not My Hand!");
        }
        else
        {
            Debug.Log("My Hand!");

            controls = new PlayerInputMap();
            input.onActionTriggered += Input_OnActionTriggered;
        }
    }

    private void Input_OnActionTriggered(CallbackContext obj)
    {
        if (!gameObject.activeSelf) { return; }
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

    [ServerRpc(RequireOwnership = false)]
    public void SwitchToStateServerRpc(int newState, int OwnerId)
    {
        SwitchToStateClientRpc(newState, OwnerId);
    }

    [ClientRpc]
    private void SwitchToStateClientRpc(int newState, int OwnerId)
    {
        Debug.Log("Switching State Visual!");
        Debug.Log(GetComponent<HandID>().ID);

        if (OwnerId != PlayerInteractionManager.Instance.GetPlayerId()) 
        {
            controller.SetCurrentState(newState, false);
            Debug.Log("SwitchSuccess: " + newState);
        }
    }

    
    [ServerRpc(RequireOwnership = false)]
    public void SwitchToDifferentHeldServerRpc(int newHeld, int OwnerId)
    {
        SwitchToDifferentHeldClientRpc(newHeld, OwnerId);
    }
    [ClientRpc]
    private void SwitchToDifferentHeldClientRpc(int newHeld, int OwnerId)
    {
        Debug.Log(newHeld);
        if (OwnerId != PlayerInteractionManager.Instance.GetPlayerId()) 
        {
            Debug.Log("SettingHeld");
            controller.SetHeld(newHeld);
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
