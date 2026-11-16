using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class OnlinePlayerInputHandler : PlayerInputHandler
{
    [SerializeField] NetworkObject networkObject;
    PlayerInput onlinePlayerInput;
    public void Start()
    {
        if (networkObject.IsLocalPlayer)
        {
            onlinePlayerInput = gameObject.GetComponent<PlayerInput>();
            onlinePlayerInput.onActionTriggered += base.Input_onActionTriggered;
        }
    }
}
