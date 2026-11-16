using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class PlayerInteractionManager : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] GameObject playerParticleObject;
    [SerializeField] GameObject playerPointerObject;
    [SerializeField] GameObject playerOverviewObject;
    [SerializeField] GameObject playerHandObject;
    [SerializeField] GameObject playerScoreObject;

    [SerializeField] Transform playerPointerHolder;
    [SerializeField] RectTransform playerOverviewHolder;
    [SerializeField] Transform playerScoreBoard;
    [SerializeField] Transform playerHandHolder;

    [SerializeField] RectTransform selectableObjectsHolder;
    [SerializeField] GameObject HandCanvas;

    [SerializeField] List<Transform> SpawnLocations;

    [SerializeField] float maxSqrGrabPlayerDistance; //distance on ServerSide, to keep more fairness to grabbed player (no abuse with distance on grabber, like "bad" network connection)
    [SerializeField] CinemachineTargetGroup targetGroup;
    [SerializeField] Transform PlayerPrefab;
    public static PlayerInteractionManager Instance { get; private set; }



    //Dictionaries are ID and Player Game Objects
    private Dictionary<int, GameObject> playerIdGoDictionary;
    private Dictionary<int, GameObject> playerParticlesIdGoDictionary;
    private Dictionary<int, GameObject> playerHandIdGoDictionary;
    private Dictionary<int, GameObject> playerPointerDictionary;
    private Dictionary<int, GameObject> playerOverviewDictionary;
    private Dictionary<int, GameObject> playerScoreUIDictionary;



    private int PlayerCount = 0;
    private int myId = 0; //note that the PiD should be a value that fits within the size of the list of players and corresponds to their position in the list.
    private GameObject LocalPlayer;
    private GameObject LocalScore;
    private GameObject LocalPlayerPointer;

    public int FinishedCounter = 0;
    public int FinishedLocalCounter = 0;

    public event EventHandler FinishedSetupEvent;
    void Awake()
    {
        Instance = this;
        //playerParticlesIdGoDictionary = new Dictionary<int, GameObject>();
        playerPointerDictionary = new Dictionary<int, GameObject>();
        playerIdGoDictionary = new Dictionary<int, GameObject>();
        playerHandIdGoDictionary = new Dictionary<int, GameObject>();
        playerParticlesIdGoDictionary = new Dictionary<int, GameObject>();
        myId = -1;
        PlayerCount = 0;
    }
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += NetworkManager_OnLoadEventCompleted;

            
        }

    }
    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {

    }

    private void NetworkManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        foreach (ulong clientID in clientsCompleted)
        {
            Transform player = Instantiate(PlayerPrefab);
            player.SetPositionAndRotation(SpawnLocations[OnlineGameFlowManager.Instance.GetPIdFromClientId(clientID)].position, player.rotation);
            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientID, true);
        }
    }
    public void JoinDictionaryPlayer(OnlinePlayer p)
    {
        if (playerIdGoDictionary == null)
        {
            playerIdGoDictionary = new Dictionary<int, GameObject>();
        }
        playerIdGoDictionary.Add(p.ID, p.gameObject);

        //p.gameObject.GetComponent<PlayerController>().SetRespawnPoint(SpawnLocations[p.ID]);
        //playerParticlesIdGoDictionary.Clear();
        //playerIdGoDictionary.Clear();

        //var particles = GameObject.FindGameObjectsWithTag("PlayerParticleSys").ToList();

        //foreach (var p in particles)
        //{
        //    playerParticlesIdGoDictionary.Add(p.GetComponent<ObjectID>().ID, p);
        //    Debug.Log(p.GetComponent<ObjectID>().ID, p);
        //}

        //var players = GameObject.FindGameObjectsWithTag("Player").ToList();

        //foreach (var p in players)
        //{
        //    if (!playerIdGoDictionary.ContainsKey(p.GetComponent<OnlinePlayer>().ID))
        //    {
        //        playerIdGoDictionary.Add(p.GetComponent<OnlinePlayer>().ID, p);
        //    }
        //    Debug.Log(p.GetComponent<OnlinePlayer>().ID);
        //}
    }

    public void JoinDictionaryParticle(GameObject particle)
    {
        if (playerParticlesIdGoDictionary == null)
        {
            playerParticlesIdGoDictionary = new Dictionary<int, GameObject>();
        }
        playerParticlesIdGoDictionary.Add(particle.GetComponent<ParticleID>().ID, particle);

        int colorId = OnlineGameFlowManager.Instance.GetPlayerDataFromPlayerIndex(particle.GetComponent<ParticleID>().ID).colorId;

        particle.GetComponent<PlayerParticles>().SetColor(OnlineGameFlowManager.Instance.GetPlayerColor(colorId));
    } 
    public void StartPlacingHands()
    {

        foreach(KeyValuePair<int, GameObject> handKvp in playerHandIdGoDictionary)
        {
            handKvp.Value.GetComponent<HandControllerOnline>().StartPlacing();
        }
        
    }
    public void SetPlacablesHands(List<GameObject> placables)
    {
        Debug.Log(playerHandIdGoDictionary.Count + " hands in dict");
        foreach (KeyValuePair<int, GameObject> handKvp in playerHandIdGoDictionary)
        {
            handKvp.Value.GetComponent<HandControllerOnline>().SetPlaceableObjectList(placables);
        }
    }

    public void JoinDictionaryHand(GameObject hand)
    {
        if (playerHandIdGoDictionary == null)
        {
            playerHandIdGoDictionary = new Dictionary<int, GameObject>();
        }

        playerHandIdGoDictionary.Add(hand.GetComponent<HandID>().ID, hand);

        int colorId = OnlineGameFlowManager.Instance.GetPlayerDataFromPlayerIndex(hand.GetComponent<HandID>().ID).colorId;

        hand.GetComponent<HandleHandInputsOnline>().InitializePlayerHandServerRpc(colorId, hand.GetComponent<HandID>().ID);

        hand.GetComponent<HandControllerOnline>().HideHand();
    }


     public void InitializeLocalPlayer(GameObject player)
     {
        LocalPlayer = player;
        targetGroup.AddMember(player.transform, 1,1.5f);
        myId = player.GetComponent<OnlinePlayer>().ID;

        ServerGameManager.Instance.LocalPlayer = player;

        InitPlayerNetworkObjectsServerRpc(player.GetComponent<OnlinePlayer>().ID);

        FinishedPlayerSetupServerRpc();

        player.GetComponent<PlayerController>().CanControl(false);
        //player.GetComponent<PlayerController>().SetRespawnPoint(SpawnLocations[myId]);

        player.transform.position = SpawnLocations[myId].position;
        //InitPlayerLocalObjectsServerRpc(player.GetComponent<OnlinePlayer>().ID);
     }

    
    [ServerRpc(RequireOwnership = false)]
    private void FinishedPlayerSetupServerRpc()
    {
        FinishedCounter++;

        if(FinishedCounter == OnlineGameFlowManager.Instance.GetPlayerCount())
        {
            foreach(KeyValuePair<int,GameObject> keyValuePair in playerIdGoDictionary)
            {
                InitPlayerLocalObjectsClientRpc(keyValuePair.Key);
            }
        }
    }
    [ServerRpc(RequireOwnership =false)]
    private void FinishedLocalSetupServerRpc()
    {
        FinishedLocalCounter++;

        if (FinishedLocalCounter == OnlineGameFlowManager.Instance.GetPlayerCount())
        {
            FinishedSetupEvent.Invoke(this, EventArgs.Empty);
        }
    }
    public Transform GetIDSpawnTransform(int playerId)
    {
        return SpawnLocations[playerId];
    }

    [ServerRpc(RequireOwnership = false)]
    private void InitPlayerNetworkObjectsServerRpc(int pID) {

        PlayerCount++;

        GameObject _playerParticleObject = Instantiate(playerParticleObject);

        _playerParticleObject.GetComponent<NetworkObject>().Spawn(true);
        _playerParticleObject.GetComponent<ParticleID>().SetIDServerRpc(pID);

        ulong id = OnlineGameFlowManager.Instance.GetClientIdFromPId(pID);
        GameObject _playerHandObject = Instantiate(playerHandObject);
        _playerHandObject.GetComponent<NetworkObject>().SpawnWithOwnership(id, true);
        _playerHandObject.GetComponent<HandID>().SetIDServerRpc(pID);

        //int color = OnlineGameFlowManager.Instance.GetPlayerDataFromPlayerIndex(pID).colorId;

        //_playerParticleObject.GetComponent<PlayerParticles>().SetColor(OnlineGameFlowManager.Instance.GetPlayerColor(color));
    }

    [ServerRpc(RequireOwnership = false)]
    public void InitPlayerLocalObjectsServerRpc(int pID)
    {
        InitPlayerLocalObjectsClientRpc(pID);
    }
    [ClientRpc]
    private void InitPlayerLocalObjectsClientRpc(int pID)
    {
            
        int colorId = OnlineGameFlowManager.Instance.GetPlayerDataFromPlayerIndex(pID).colorId;
        int faceId = OnlineGameFlowManager.Instance.GetPlayerDataFromPlayerIndex(pID).faceId;
        Color color = OnlineGameFlowManager.Instance.GetPlayerColor(colorId);
        Texture2D face = OnlineGameFlowManager.Instance.GetPlayerFaceTexture(faceId);

        string name = "P" + (pID + 1).ToString();
        //PlayerPointer
        if (playerPointerDictionary == null)
        {
            playerPointerDictionary = new Dictionary<int, GameObject> ();
        }

        var playerPointer = Instantiate(playerPointerObject, playerPointerHolder);

        playerPointer.GetComponent<PointerScript>().SetPlayer(playerIdGoDictionary[pID].transform);

        playerPointer.GetComponent<PointerScript>().SetColor(color);

        playerPointer.GetComponent<PointerScript>().SetName(name);
        playerPointer.GetComponentInChildren<SpeechScript>().SetIsOnline(true);


        if (pID == myId)
        {
            LocalPlayer.GetComponent<PlayerInputHandler>().SetSpeechScript(playerPointer.GetComponentInChildren<SpeechScript>());
            LocalPlayer.GetComponent<PlayerManager>().SetPlayerPointer(playerPointer);
        }

        playerPointerDictionary.Add(pID, playerPointer);

        //PlayerOverview
        if (playerOverviewDictionary == null)
        {
            playerOverviewDictionary = new Dictionary<int, GameObject>();
        }

        var playerOverview = Instantiate(playerOverviewObject, playerOverviewHolder);

        playerOverview.GetComponent<UiStatsScript>().SetColor(color);
        playerOverview.GetComponent<UiStatsScript>().SetPlayerName(name);
        playerOverview.GetComponent<UiStatsScript>().SetFace(face);

        playerOverview.GetComponent<UiStatsScript>().SetIsOnline(true);


        playerOverview.GetComponent<UiStatsScript>().SetHealthLocal(LocalPlayer.GetComponent<PlayerManager>().StartHP);

        if (pID == myId)
        {
            LocalPlayer.GetComponent<PlayerManager>().SetUIStatItem(playerOverview.GetComponent<UiStatsScript>());
        }

        playerOverviewDictionary.Add(pID, playerOverview);


        if (playerScoreUIDictionary == null)
        {
            playerScoreUIDictionary = new Dictionary<int, GameObject>();
        }

        var playerScoreVisual = Instantiate(playerScoreObject, playerScoreBoard);

        playerScoreVisual.GetComponent<ScoreItemScript>().InitializeOnline(name, color, face);

        playerScoreUIDictionary.Add(pID, playerScoreVisual);
        if (pID != myId)
        {
            playerIdGoDictionary[pID].GetComponent<OnlinePlayer>().DisableScripts();
        }
        //playerScoreVisual.GetComponent<ScoreItemScript>().

        if (IsServer)
        {
            foreach (KeyValuePair<int, GameObject> kvp in playerHandIdGoDictionary)
            {

                kvp.Value.transform.SetParent(HandCanvas.transform, false);
            }
        }


        FinishedLocalSetupServerRpc();
    }
    //public void RequestPlayerUIStart(bool allFinished)
    //{
    //    foreach(KeyValuePair<int, GameObject> kvp in playerScoreUIDictionary)
    //    {
    //        kvp.Value.GetComponent<ScoreItemScript>().StartPhaseOnline(allFinished);
    //        kvp.Value.GetComponent<ScoreItemScript>().StartAnim();
    //    }
    //}
    //Particle Action
    public void RequestParticleAction(Vector3 pos, int type, int playerID)
    {
        ParticleActionServerRpc(pos, type, playerID);
    }


    [ServerRpc(RequireOwnership =false)]
    private void ParticleActionServerRpc(Vector3 pos, int type, int playerID)
    {
        if (pos == null) { return; }
        ParticleActionClientRpc(pos, type, playerID);
    }
    [ClientRpc]
    private void ParticleActionClientRpc(Vector3 pos, int type, int playerID)
    {
        
        if (myId != playerID)
        {
            DoParticles(pos, type, playerID);
        }
    }

    private void DoParticles(Vector3 pos, int type, int playerID)
    {
        if (playerID >= 0 && playerID < playerParticlesIdGoDictionary.Count)
        {
            playerParticlesIdGoDictionary[playerID].GetComponent<PlayerParticles>().DoParticles(pos, type);
        }
    }
    
    //Attack Action
    public void RequestAttackAction(int playerID) { AttackActionServerRpc(playerID); }
    [ServerRpc(RequireOwnership =false)]
    private void AttackActionServerRpc(int playerID)
    {
        AttackActionClientRpc(playerID);
    }
    [ClientRpc]
    private void AttackActionClientRpc(int playerID) {
        if (myId != playerID && playerID >= 0 && playerID < playerIdGoDictionary.Count)
        {
            playerIdGoDictionary[playerID].GetComponentInChildren<PlayerAttackScript>().InitAttack();
        }
    }

    public void RequestDisableVisual()
    {
        DisablePlayerVisualServerRpc(myId);
    }
    [ServerRpc(RequireOwnership = false)]
    private void DisablePlayerVisualServerRpc(int id)
    {
        DisablePlayerVisualClientRpc(id);
    }
    [ClientRpc]
    private void DisablePlayerVisualClientRpc(int id)
    {
        if(myId == id) { return; }
        playerPointerDictionary[id].SetActive(false);
        playerParticlesIdGoDictionary[id].SetActive(false);
    }
    //Grab Action
    public void RequestPlayerGrab(int playerID, int grabbedID) 
    { 
        RequestPlayerGrabServerRpc(playerID, grabbedID);
    }
    [ServerRpc(RequireOwnership = false)]
    private void RequestPlayerGrabServerRpc(int playerID, int grabbedID) {
        if((playerID >= 0 && playerID < playerIdGoDictionary.Count) && (grabbedID >= 0 && grabbedID < playerIdGoDictionary.Count))
        {
            if ((playerIdGoDictionary[grabbedID].transform.position - playerIdGoDictionary[playerID].transform.position).sqrMagnitude <= maxSqrGrabPlayerDistance) {
                RequestPlayerGrabClientRpc(playerID, grabbedID);
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void RemovePlayerServerRpc(int playerID)
    {
        RemovePlayerClientRpc(playerID);
    }

    [ClientRpc(RequireOwnership = false)]
    private void RemovePlayerClientRpc(int playerID)
    {
        //playerIdGoDictionary[playerID].GetComponent<OnlineGrab>().Release();

        Destroy(playerPointerDictionary[playerID]);
        playerPointerDictionary.Remove(playerID);


        Destroy(playerIdGoDictionary[playerID]);
        playerIdGoDictionary.Remove(playerID);

        Destroy(playerOverviewDictionary[playerID]);
        playerOverviewDictionary.Remove(playerID);

        playerParticlesIdGoDictionary.Remove(playerID);
        playerHandIdGoDictionary.Remove(playerID);
    }
    [ClientRpc]
    private void RequestPlayerGrabClientRpc(int playerID, int grabbedID) {

        Debug.Log("Player " + playerID + " Grab" + grabbedID);
        if (playerID == myId)
        {
            //succesfull Grab grabber Side
            playerIdGoDictionary[playerID].GetComponentInChildren<OnlineGrab>().RecieveGrabConfirm();
        }else //if (grabbedID == PiD)
        {
            //Grab on grabbed side1
            playerIdGoDictionary[playerID].GetComponentInChildren<OnlineGrab>().ForceGrab(playerIdGoDictionary[grabbedID]);
        }
    }


    //Release action
    public void RequestPlayerRelease(int playerID, int grabbedID) { RequestPlayerReleaseServerRpc(playerID, grabbedID); }
    [ServerRpc(RequireOwnership = false)]
    private void RequestPlayerReleaseServerRpc(int playerID, int grabbedID) {
        RequestPlayerReleaseClientRpc(playerID, grabbedID);
    }
    [ClientRpc]
    private void RequestPlayerReleaseClientRpc(int playerID, int grabbedID)
    {
        playerIdGoDictionary[playerID].GetComponentInChildren<OnlineGrab>().Release();
    }

    //Throw action
    public void RequestPlayerThrow(int playerID, int grabbedID) { RequestPlayerThrowServerRpc(playerID, grabbedID); }
    [ServerRpc(RequireOwnership = false)]
    private void RequestPlayerThrowServerRpc(int playerID, int grabbedID) { RequestPlayerThrowClientRpc(playerID, grabbedID); }
    [ClientRpc]
    private void RequestPlayerThrowClientRpc(int playerID, int grabbedID) { playerIdGoDictionary[playerID].GetComponentInChildren<OnlineGrab>().Throw(); }



    //Speak action
    public void RequestPlayerSpeak(int emoteId)
    {
        RequestPlayerSpeakServerRpc(myId, emoteId);
    }
    [ServerRpc(RequireOwnership = false)]
    private void RequestPlayerSpeakServerRpc(int playerID, int emoteId) {
        RequestPlayerSpeakClientRpc(playerID, emoteId);
    }    
    [ClientRpc]
    private void RequestPlayerSpeakClientRpc(int playerID, int emoteId) {
        if(playerID != myId)
        {
            playerPointerDictionary[playerID].transform.GetComponentInChildren<SpeechScript>().Speak(emoteId);
        }
    }

    //Hp Change action
    public void RequestHpChange(int hpChange) {
        RequestHpChangeServerRpc(hpChange, myId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestHpChangeServerRpc(int newHp, int pID) { 
        ChangeHpClientRpc(newHp, pID);
    }

    [ClientRpc]
    private void ChangeHpClientRpc(int newHp, int pID) {

        if (pID != myId)
        {
            playerOverviewDictionary[pID].GetComponent<UiStatsScript>().SetHealthLocal(newHp);

            if (newHp <= 0)
            {
                playerIdGoDictionary[pID].GetComponent<PlayerController>().TurnOnRagdoll();
                playerIdGoDictionary[pID].GetComponent<PlayerManager>().EnableGrave(true);

            }
        }
    }

    public void DespawnPlayer()
    {
        DespawnPlayerServerRpc(myId);
    }
    [ServerRpc(RequireOwnership = false)]
    private void DespawnPlayerServerRpc(int playerId)
    {
        DespawnPlayerClientRpc(playerId);
    }
    [ClientRpc]
    private void DespawnPlayerClientRpc(int playerId) 
    {
        playerPointerDictionary[playerId].SetActive(false);
        playerIdGoDictionary[playerId].SetActive(false);
        playerIdGoDictionary[playerId].GetComponentInChildren<OnlineMaterialChanger>().HideDecal();
    }

    [ServerRpc]
    public void RespawnAllPlayersServerRpc()
    {
        //foreach(KeyValuePair<int, GameObject> kvp in playerPointerDictionary)
        //{
        //    kvp.Value.SetActive(true);
        //}        
        //foreach(KeyValuePair<int, GameObject> kvp in playerIdGoDictionary)
        //{
        //    kvp.Value.SetActive(true);
        //    kvp.Value.GetComponentInChildren<OnlineMaterialChanger>().ShowDecal();
        //    //kvp.Value.GetComponent<PlayerController>().Respawn();

        //    kvp.Value.transform.SetPositionAndRotation(SpawnLocations[kvp.Key].position, kvp.Value.transform.rotation);
        //    kvp.Value.transform.SetLocalPositionAndRotation(Vector3.zero, kvp.Value.transform.localRotation);
        //    kvp.Value.transform.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        //    kvp.Value.transform.localPosition = Vector3.zero;
        //}

        RespawnAllPlayersClientRpc();
        ShowPlayerObjectsClientRpc();
    }
    [ClientRpc]
    private void RespawnAllPlayersClientRpc()
    {
        //for local Per Player things, so far only ragdoll
        foreach (KeyValuePair<int, GameObject> kvp in playerIdGoDictionary)
        {
            kvp.Value.GetComponent<PlayerManager>().EnableGrave(false);

            kvp.Value.transform.SetPositionAndRotation(SpawnLocations[kvp.Key].position, kvp.Value.transform.rotation);
            kvp.Value.transform.SetLocalPositionAndRotation(Vector3.zero, kvp.Value.transform.localRotation);
            kvp.Value.transform.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;

            kvp.Value.GetComponent<PlayerManager>().SetHP(kvp.Value.GetComponent<PlayerManager>().StartHP);
            kvp.Value.GetComponent<PlayerController>().TurnOffRagdoll();
            kvp.Value.GetComponent<PlayerManager>().EnableGrave(false);
        }

    }
    [ClientRpc]
    private void ShowPlayerObjectsClientRpc()
    {
        foreach(KeyValuePair<int, GameObject> keyValuePair in playerPointerDictionary)
        {
            if (keyValuePair.Value == null)
            {
                continue;
            }
            keyValuePair.Value.SetActive(true);
        }
        foreach(KeyValuePair<int, GameObject> keyValuePair in playerParticlesIdGoDictionary)
        {
            if(keyValuePair.Value == null)
            {
                continue;
            }
            keyValuePair.Value.SetActive(true);
        }
    }
    public GameObject GetLocalPlayer()
    {
        return LocalPlayer;
    }
    public void InitPlayerStatsItem()
    {
        //for getting Placement
        List<int> playerPoints = new List<int>();
        foreach(KeyValuePair<int, GameObject> kvp in playerScoreUIDictionary)
        {
            playerPoints.Add(ServerGameManager.Instance.GetPlayerPointsThisRound(kvp.Key));
            Debug.Log(ServerGameManager.Instance.GetPlayerPointsThisRound(kvp.Key) + " My Points P" + kvp.Key);
        }

        playerPoints.Sort();
        playerPoints.Reverse();

        foreach(KeyValuePair<int , GameObject> scorerItem in playerScoreUIDictionary)
        {
            Debug.Log(scorerItem.Key);
            scorerItem.Value.GetComponent<ScoreItemScript>().SetTimeThisRound(ServerGameManager.Instance.GetPlayerTimeTaken(scorerItem.Key));

            int myPoints = ServerGameManager.Instance.GetPlayerPointsThisRound(scorerItem.Key);

            scorerItem.Value.GetComponent<ScoreItemScript>().SetPointsThisRound(myPoints, playerPoints.IndexOf(myPoints));
        }
        Debug.Log("Finished Point Tally");
        Debug.Log(playerScoreUIDictionary.Count + " Score Ui Count");
    }
    public void StartPlayerStatsItem(bool allFinished)
    {
        Debug.Log("Started Animation");
        foreach (KeyValuePair<int, GameObject> scorerItem in playerScoreUIDictionary)
        {
            scorerItem.Value.GetComponent<ScoreItemScript>().StartPhaseOnline(allFinished);
            scorerItem.Value.GetComponent<ScoreItemScript>().StartAnim();
        }
    }
    public int GetPlayerId()
    {
        return myId;
    }
    public RectTransform GetSelectableObjectsHolder()
    {
        return selectableObjectsHolder;
    }
}
