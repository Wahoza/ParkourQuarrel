using System.Collections.Generic;
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnlineGameFlowManager : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public static OnlineGameFlowManager Instance;

    [SerializeField] private bool hasMaxPlayers;
    public static int maxPlayerCount { get; private set; } = 5;
    [SerializeField] private List<Color> playerColorList;
    [SerializeField] private List<Texture2D> playerFaceList;

    public event EventHandler OnTryingToJoinGame;
    public event EventHandler OnFailedToJoinGame;
    public event EventHandler OnPlayerDataNetworkListChanged;

    private NetworkList<PlayerData> playerDataNetworkList;

    void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(gameObject);

        playerDataNetworkList = new NetworkList<PlayerData>();

        playerDataNetworkList.OnListChanged += PlayerData_OnListChanged;
    }
    
    private void PlayerData_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
    }
    public bool IsPlayerIndexConnected(int playerIndex)
    {
        return playerIndex < playerDataNetworkList.Count;
    }

    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartHost();
    }

    private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientId)
    {
        for (int i = 0; i < playerDataNetworkList.Count; i++)
        {
            PlayerData playerData = playerDataNetworkList[i];
            if(playerData.clientId == clientId)
            {

                playerDataNetworkList.RemoveAt(i);
            }
        }    
    }

    private void NetworkManager_OnClientConnectedCallback(ulong clientId)
    {
        playerDataNetworkList.Add(new PlayerData
        {
            clientId = clientId,
            colorId = GetFirstUnusedColorID(),
            faceId = 0
        });
    }

    public void StartClient()
    {
        OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);

        NetworkManager.Singleton.OnClientDisconnectCallback += NetwokrManager_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartClient();
    }

    private void NetwokrManager_OnClientDisconnectCallback(ulong clientID)
    {
        OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);
    }

    public void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse response)
    {
        if(IsHost || IsServer) {
            response.Approved = true;

            return; }
        if (SceneManager.GetActiveScene().name == Loader.Scenes.GameSceneOnline.ToString()) //isWaitingToStart
        {
            response.Approved = false;
            response.Reason = "Game has already started";

            return;
        }

        if (!(!hasMaxPlayers || NetworkManager.Singleton.ConnectedClientsIds.Count < maxPlayerCount))
        {
            response.Approved = false;
            response.Reason = "Game if full";

            return;
        }

        response.Approved = true;
        response.CreatePlayerObject = true;
    }

    public PlayerData GetPlayerDataFromPlayerIndex(int playerIndex)
    {
        return playerDataNetworkList[playerIndex];
    }
    public PlayerData GetPlayerDataFromClientId(ulong clientId)
    {
        foreach (PlayerData data in playerDataNetworkList)
        {
            if (data.clientId == clientId)
            {
                return data;
            }
        }
        return default;
    }    
    public int GetPlayerCount() {  return playerDataNetworkList.Count; }
    public int GetPIdFromClientId(ulong clientId)
    {
        for (int i = 0; i<playerDataNetworkList.Count; i++)
        {
            if(playerDataNetworkList[i].clientId == clientId)
            {
                return i;
            }
        }
        return -1;
    }
    public PlayerData GetPlayerData()
    {
        return GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
    }
    public Color GetPlayerColor(int colorID)
    {
        return playerColorList[colorID];
    }
    public void SetPlayerColor(int colorId) {
        ChangePlayerColorServerRpc(colorId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangePlayerColorServerRpc(int colorId, ServerRpcParams serverRpcParams = default) 
    {
        if (!IsColorAvailible(colorId))
        {
            return;
        }
        int playerDataId = GetPIdFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData data = playerDataNetworkList[playerDataId];
        data.colorId = colorId;

        playerDataNetworkList[playerDataId] = data;
    }
    public Texture2D GetPlayerFaceTexture(int textureId)
    {
        return playerFaceList[textureId];
    }

    public void SetPlayerFace(int faceId)
    {
        ChangePlayerFaceServerRpc(faceId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangePlayerFaceServerRpc(int faceId, ServerRpcParams serverRpcParams = default) {
        int playerDataId = GetPIdFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData data = playerDataNetworkList[playerDataId];
        data.faceId = faceId;

        playerDataNetworkList[playerDataId]= data;    
    }
    private bool IsColorAvailible(int colorId)
    {
        foreach (PlayerData data in playerDataNetworkList)
        {
            if (data.colorId == colorId)
            {
                return false;
            }
        }
        return true;
    }
    private int GetFirstUnusedColorID()
    {
        for(int i = 0; i < playerColorList.Count; i++)
        {
            if (IsColorAvailible(i))
            {
                return i;
            }
        }
        return -1;
    }

    public void KickPlayer(ulong clientId)
    {
        NetworkManager.Singleton.DisconnectClient(clientId);
        NetworkManager_Server_OnClientDisconnectCallback(clientId);
    }
    public ulong GetClientIdFromPId(int pId)
    {
        if(playerDataNetworkList.Count > pId)
        {
            return playerDataNetworkList[pId].clientId;
        }
        return 0;
    }
}
