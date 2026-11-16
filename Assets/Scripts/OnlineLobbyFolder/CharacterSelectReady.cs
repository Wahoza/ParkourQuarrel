using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterSelectReady : NetworkBehaviour
{
    [SerializeField] ServerGameSettings gameSettings;

    private Dictionary<ulong, bool> playerReadyDictionary;

    public event EventHandler OnReadyChangend;
    public static CharacterSelectReady Instance { get; private set; }
    
    private void Awake()
    {
        Instance = this;
        playerReadyDictionary = new Dictionary<ulong, bool>();
    }

    public void SetPlayerReady(bool ready) //call this on button
    {
        if (ready)
        {
            SetPlayerReadyServerRpc();
        }
        else
        {
            SetPlayerUnreadyServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        SetPlayerRadyClientRpc(serverRpcParams.Receive.SenderClientId);

        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        bool allClientsReady = true;
        foreach (ulong clientID in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerReadyDictionary.ContainsKey(clientID) || !playerReadyDictionary[clientID])
            {
                allClientsReady = false;
                break;
            }
        }

        if (allClientsReady)
        {
            //start
            gameSettings.ConfirmSettings();
            Loader.LoadNetwork(Loader.Scenes.GameSceneOnline);
        }
    }
    [ServerRpc(RequireOwnership = false)]

    private void SetPlayerUnreadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        SetPlayerUnreadyClientRpc(serverRpcParams.Receive.SenderClientId);

        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = false;
    }

    [ClientRpc] 
    private void SetPlayerRadyClientRpc(ulong clientID)
    {
        playerReadyDictionary[clientID] = true;

        OnReadyChangend?.Invoke(this, EventArgs.Empty);
    }
    
    [ClientRpc]
    private void SetPlayerUnreadyClientRpc(ulong clientID)
    {
        playerReadyDictionary[clientID] = false;

        OnReadyChangend?.Invoke(this, EventArgs.Empty);
    }

    public bool isPlayerReady(ulong clientID)
    {
        if (playerReadyDictionary.ContainsKey(clientID))
        {
            return playerReadyDictionary[clientID];
        }
       return false;
    }
}
