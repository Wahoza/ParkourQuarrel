using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class OnlinePlayer : NetworkBehaviour
{
    [SerializeField] NetworkObject networkObject;
    [SerializeField] List<MonoBehaviour> ScriptsToDisable;
    [SerializeField] GameObject particlesPrefab;
    [SerializeField] PlayerController playerController;
    [SerializeField] PlayerManager playerManager;
    [SerializeField] OnlineMaterialChanger materialChanger;

    public bool isHostPlayer { get; private set; }  = false;

    public int ID { get; private set; }
    public bool isFinished { get; private set; }
    public PlayerState currentState { get; private set; }
    public enum PlayerState
    {
        alive = 0,
        finished = 1
    }

    private void Start()
    {
        PlayerData playerData = OnlineGameFlowManager.Instance.GetPlayerDataFromClientId(OwnerClientId);
        materialChanger.SetPlayerColor(OnlineGameFlowManager.Instance.GetPlayerColor(playerData.colorId));
        materialChanger.SetFace(OnlineGameFlowManager.Instance.GetPlayerFaceTexture(playerData.faceId));

        playerManager.SetID(OnlineGameFlowManager.Instance.GetPIdFromClientId(OwnerClientId));
        ID = OnlineGameFlowManager.Instance.GetPIdFromClientId(OwnerClientId);

        PlayerInteractionManager.Instance.JoinDictionaryPlayer(this);

        if (!networkObject.IsLocalPlayer)
        {
            foreach (MonoBehaviour behaviour in ScriptsToDisable) { behaviour.enabled = false; } //to disable Scipts And Only Have ONE LocalPlayer
        }
        else
        {
            //ServerGameManager.Instance.SetLocalPlayer(gameObject);#
            //playerManager.SetID(networkObject.GetInstanceID());

            PlayerInteractionManager.Instance.InitializeLocalPlayer(gameObject);
            ServerGameManager.Instance.GetPlayersServerRpc();
            ServerGameManager.Instance.SetLocalPlayer(gameObject);
        }
    }
    public void DisableScripts()
    {
        foreach (MonoBehaviour behaviour in ScriptsToDisable) { behaviour.enabled = false; }
    }
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
            isHostPlayer = true;
        }
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        if(clientId == OwnerClientId)
        {
            PlayerInteractionManager.Instance.RemovePlayerServerRpc(ID);
            //Here: Release Player
            //Destroy other Player related things (pointer, Hand, ScoreItem) (probably can call this on mainGame)
            //pausing will require a boolean
            //Remove from PlayerList (for finishedChecking)
            //Also unPause
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateStateServerRpc(int state)
    {
        UpdateStateClientRpc(state);
    }
    [ClientRpc]
    private void UpdateStateClientRpc(int state)
    {
        currentState = (PlayerState)state;
    }

    public void UpdateState(int state)
    {
        currentState = (PlayerState)state;
    }
    public void Respawn()
    {
        RespawnServerRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    private void RespawnServerRpc()
    {
        isFinished = false;
    }
    public void FinishInstance() {

        //ServerGameManager.Instance.FinishPlayerCamSwap();
        //FinishInstanceServerRpc(); 
    }
    //[ServerRpc(RequireOwnership = false)]
    //private void FinishInstanceServerRpc()
    //{
    //    isFinished = true;

    //    //ServerGameManager.Instance.CheckFinishedServerRpc();
    //}
}
