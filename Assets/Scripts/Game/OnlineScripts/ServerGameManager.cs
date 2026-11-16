using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServerGameManager : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public enum State
    {
        parkourState = 0,
        statsState = 1,
        placingState = 2,
        countdownState = 3
    }
    public State currentState = State.countdownState;
    List<GameObject> players;
    List<HandControllerOnline> hands;
    List<PlayerManager> playerManagers;
    List<OnlinePlayer> onlinePlayers;

    [Header("ParkourStuff")]
    [SerializeField] GameObject ParkourCamera;
    [SerializeField] GameObject ParkourHud;
    [SerializeField] GameObject Goal;
    [SerializeField] TimerScript Timer;

    private List<int> finishedPlayers;
    private int ScorePlayerCounter = 0;

    [Header("PlacingStuff")]
    [SerializeField] GameObject PlacingCamera;
    [SerializeField] GameObject PlacingHud;
    [SerializeField] GameObject PlacableHud;
    [SerializeField] List<GameObject> InitialPlacedItems;
    [SerializeField] PlacableObjectsThisScene PlaceableManager;
    [SerializeField] int placableCount;

    [Header("ScoreStuff")]
    [SerializeField] ScoreHudScript ScoreHud;
    [SerializeField] ScoreBoardScript ScoreBoard;
    List<ScoreItemScript> scoreScripts;
    [SerializeField] int maxConsecutiveFails;
    [SerializeField] int maxFinishPlayers;
    bool allFinished;

    int consecutiveFails = 0;

    [Header("CountdownStuff")]
    [SerializeField] GameObject CountdownHud;
    [SerializeField] CountdownScript countdownScript;
    [SerializeField] GameObject initCam;

    [Header("OutroStuff")]
    [SerializeField] GameObject OutroHud;
    [SerializeField] EndGameScript OutroScript;

    [Header("IntroStuff")]
    [SerializeField] Animator IntroAnim;
    private bool isFirstSetup = true;
    public static ServerGameManager Instance { get; private set; }
    public GameObject LocalPlayer;

    private Dictionary<int, int> playerIdPoints;
    private Dictionary<int, int> playerIdPointsThisRound;
    private Dictionary<int, float> playerIdTimeTaken;

    [Header("Spawning Stuff")]
    [SerializeField] GameObject placedItemPrefab;
    private Dictionary<int, GameObject> spawnedObjects;
    int spawnedObjectCounter = 0;
    public void Start()
    {
        if (Instance != null)
        {
            //Debug.Log("Trying to create new instance");
        }
        else
        {
            Instance = this;
            //DontDestroyOnLoad(Instance);
            players = new List<GameObject>();
            playerManagers = new List<PlayerManager>();
            onlinePlayers = new List<OnlinePlayer>();
            hands = new List<HandControllerOnline>();
            scoreScripts = new List<ScoreItemScript>();
            spawnedObjects = new Dictionary<int, GameObject>(); 
            finishedPlayers = new List<int>();
            consecutiveFails = 0;
            allFinished = false;


            currentState = State.countdownState;
            //InitializeCountdownState();

            PlayerInteractionManager.Instance.FinishedSetupEvent += Instance_FinishedSetupEvent;
            Time.timeScale = 0f;
        }
    }

    private void Instance_FinishedSetupEvent(object sender, System.EventArgs e)
    {
        Time.timeScale = 1f;
        if (IsHost)
        {
            SwitchStateClientRpc((int)State.countdownState);
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
            countdownScript.FinishedEvent += CountdownScript_FinishedEvent;
        }
        //InitializeCountdownState();
        //currentState = State.countdownState;

    }
    
    private void CountdownScript_FinishedEvent(object sender, System.EventArgs e)
    {
        //Debug.Log("hore");
        //if (NetworkManager.Singleton.IsServer)
        //{
        //    CheckFinished();
        //}
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        //InitializeCountdownState();
        SwitchStateClientRpc((int)State.countdownState);
    }

    //public void HostButtonDown()
    //{
    //    if(LocalPlayer != null)
    //    {
    //        LocalPlayer.GetComponent<PlayerController>().CanMove(false);
    //    }


    //    if (IsServer)
    //    {
    //        SwitchStateClientRpc(State.parkourState);
    //    }
    //}

    public void SetLocalPlayer(GameObject player)
    {
        LocalPlayer = player;
        Debug.Log("SetLocalPlayer!");
    }
    // Update is called once per frame
    void InitializeParkourState()
    {
        ParkourCamera.SetActive(true);
        PlacingCamera.SetActive(false);
        ParkourHud.SetActive(true);
        ScoreHud.DisableScoreBoard();
        PlacingHud.SetActive(false);
        PlacableHud.SetActive(false);
        CountdownHud.SetActive(false);

        Timer.gameObject.SetActive(true);
        Timer.StartTimer();

        playerIdPointsThisRound?.Clear();

        if (LocalPlayer != null)
        {
            LocalPlayer.GetComponent<PlayerManager>().Respawn();
            LocalPlayer.GetComponent<PlayerController>().CanControl(true);
            LocalPlayer.GetComponent<PlayerController>().CanMove(true);
        }

        //foreach (GameObject placedItem in placedItems)
        //{
        //    placedItem.GetComponent<PlacedItem>().StartParkourState();
        //}

        if (IsHost || IsServer)
        {
            foreach (GameObject placedItem in InitialPlacedItems)
            {
                placedItem.GetComponent<PlacedItemOnline>().StartParkourStateClientRpc();
            }

            foreach(KeyValuePair<int,GameObject> placedItem in spawnedObjects)
            {
                placedItem.Value.GetComponent<PlacedItemOnline>().StartParkourStateClientRpc();
            }
        }

        Goal.GetComponent<OnlineGoalScript>()?.Respawn();
    }
    void InitializeStatsState()
    {
        ParkourCamera.SetActive(false);
        PlacingCamera.SetActive(true);
        ParkourHud.SetActive(false);
        ScoreHud.EnableScoreBoard();
        PlacingHud.SetActive(false);
        PlacableHud.SetActive(false);
        Timer.gameObject.SetActive(false);
        CountdownHud.SetActive(false);

        int finishedCount = ScorePlayerCounter;

        allFinished = finishedCount == players.Count && players.Count > maxFinishPlayers;

        if (finishedCount == 0)
        {
            consecutiveFails++;
        }
        else
        {
            consecutiveFails = 0;
        }

        LocalPlayer?.GetComponent<PlayerController>()?.CanMove(false);

        ScoreBoard.StartPhase(allFinished, finishedCount == 0, maxConsecutiveFails - consecutiveFails + 1); //+1 because of string readability



        PlayerInteractionManager.Instance.InitPlayerStatsItem();


        if (consecutiveFails > maxConsecutiveFails) { ResetBoard(); }
    }
    void InitializePlacingState()
    {
        //Debug.Log("Here Placing Init");

        if (LocalPlayer != null) { 

            //LocalPlayer.GetComponent<PlayerManager>().EnableGrave(false);
            LocalPlayer.GetComponent<PlayerManager>().DisableGameObject();
        }

        ParkourCamera.SetActive(false);
        PlacingCamera.SetActive(true);
        ParkourHud.SetActive(false);
        ScoreHud.DisableScoreBoard();
        PlacingHud.SetActive(true);
        PlacableHud.SetActive(true);
        Timer.gameObject.SetActive(false);
        CountdownHud.SetActive(false);

        //foreach (GameObject placedItem in placedItems)
        //{
        //    placedItem.GetComponent<PlacedItem>().StartPlaceState();
        //}
        if(IsHost || IsServer)
        {
            foreach (GameObject placedItem in InitialPlacedItems)
            {
                placedItem.GetComponent<PlacedItemOnline>().StartPlacingStateClientRpc();
            }

            foreach (KeyValuePair<int, GameObject> placedItem in spawnedObjects)
            {
                placedItem.Value.GetComponent<PlacedItemOnline>().StartPlacingStateClientRpc();
            }
        }


        if (IsHost)
        {
            //PlaceableManager.GenerateRandomPlacables(placableCount);
            SetSelectibleItemsClientRpc();
        }
        PlayerInteractionManager.Instance.SetPlacablesHands(PlaceableManager.GetPlaceables());

        PlayerInteractionManager.Instance.StartPlacingHands();
    }

    void InitializeCountdownState()
    {
        ParkourCamera.SetActive(!countdownScript.isFirstCountdown);
        PlacingCamera.SetActive(!countdownScript.isFirstCountdown);
        ParkourHud.SetActive(false);
        ScoreHud.DisableScoreBoard();
        PlacingHud.SetActive(false);
        PlacableHud.SetActive(false);
        Timer.gameObject.SetActive(false);

        CountdownHud.SetActive(true);
        
        countdownScript.StartCountdown();

        ScorePlayerCounter = 0;

        //initCam.SetActive(false);
        if (IsHost)
        {
            PlayerInteractionManager.Instance.RespawnAllPlayersServerRpc();
        }

        Debug.Log("Respawning ...");
        if (LocalPlayer != null)
        {
            Debug.Log("Respawning with LocalPlayer");
            LocalPlayer.GetComponent<PlayerManager>().EnableGameObject();
            LocalPlayer.GetComponent<PlayerManager>().Respawn();
            //resets position for nice visual, im not the biggest fan of doing it like this
            LocalPlayer.GetComponent<PlayerController>().TurnOffRagdoll();

            LocalPlayer.GetComponent<PlayerController>().Respawn();
            LocalPlayer.GetComponent<PlayerController>().CanMove(false);
        }
        //player.GetComponent<PlayerController>().enabled = false;

        //we are doing this here, as this is hopefully the time when the least data is transmitted.
        if (IsServer)
        {
            ClearPlacableItemsClientRpc();

            PlaceableManager.GenerateRandomPlacables(placableCount);

            foreach (int id in PlaceableManager.GetPlacableIDs())
            {
                AddPlacableClientRpc(id);
            }
        }
    }
    private void CheckFinished()
    {


        bool isFinished = false;

        switch (currentState)
        {
            case State.parkourState:

                if (finishedPlayers.Count >=  OnlineGameFlowManager.Instance.GetPlayerCount() || Timer.isElapsed) //implies that the player is dead or won
                {
                    currentState = State.statsState;

                    isFinished = true;
                }
                break;
            case State.statsState:
                if (ScoreBoard.GetIsFinished())
                {
                    currentState = State.placingState;

                    isFinished = true;
                }
                break;
            case State.placingState:

                if (finishedPlayers.Count >= OnlineGameFlowManager.Instance.GetPlayerCount())//hands.All(h => h.GetCurrentState() >= 2))
                {
                    currentState = State.countdownState;


                    isFinished = true;
                }
                break;
            case State.countdownState:
                if (countdownScript.state == CountdownScript.currentState.finished) 
                {
                    currentState = State.parkourState;

                    isFinished = true;
                }
                break;
        }

        if (isFinished)
        {
            //Debug.Log("here");
            SwitchStateClientRpc((int)currentState);
        }
    }


    //public void AddPlayer(GameObject go)
    //{
    //    if (go == null) { return; }

    //    if (players.Contains(go)) { return; }
    //    players.Add(go);
    //    playerManagers.Add(go.GetComponent<PlayerManager>());
        
    //    if(currentState != State.parkourState)
    //    {
    //        go.GetComponent<PlayerController>().CanMove(false);
    //        //go.GetComponent<PlayerStats>().Respawn();

    //        //go.GetComponent<PlayerStats>().Respawn();
    //    }
    //}


    //public void AddHand(GameObject go)
    //{
    //    if (go.GetComponent<HandController>() == null) { return; }
    //    if (hands.Contains(go.GetComponent<HandController>())) { return; }

    //    hands.Add(go.GetComponent<HandController>());
    //}


    //public void AddPlacedItem(GameObject go)
    //{
    //    if (placedItems.Contains(go)) { return; }

    //    placedItems.Add(go);
    //}

    //public void RemovePlacedItem(GameObject go)
    //{
    //    if (!placedItems.Contains(go)) { return; }
    //    placedItems.Remove(go);
    //}

    public void EndGame()
    {
        foreach (ScoreItemScript script in scoreScripts) {
            script.GetFinalPoints();
        }
        OutroHud.SetActive(true);
        OutroScript.StartEnd();
        ScoreBoard.GameEnd();
    }
    public void AddScoreItem(ScoreItemScript item)
    {
        if(item == null) { return; }
        if(scoreScripts.Contains(item)) { return; }
        scoreScripts.Add(item);
    }
    public void AnimScoreboardEnd(int animation)
    {
        if (animation == 0)
        {
            //foreach (ScoreItemScript script in scoreScripts)
            //{
            //    Debug.Log(allFinished);
            //    script.StartPhase(allFinished);
            //    script.StartAnim();

            //}
            PlayerInteractionManager.Instance.StartPlayerStatsItem(allFinished);
        }
        else
        {
            ScoreBoard.EnableSwitch();
        }
    }

    private void ResetBoard()
    {
        consecutiveFails = 0;
        //Debug.Log(placedItems.Count);
        //for (int i = placedItems.Count - 1; i >= 0; i--)
        //{
        //    var item = placedItems[i];
        //    placedItems.Remove(item);
        //    item.GetComponent<PlacedItem>().StartPlaceState();
        //    Destroy(item);
        //}
    }

    public int getPlacedItemCount() {
        //return placedItems.Count();
        return 0;
    }

  
    [ServerRpc(RequireOwnership = false)]
    public void CheckFinishedServerRpc(int finisherID)
    {
        if(currentState == State.parkourState || currentState == State.placingState) {
            if (!finishedPlayers.Contains(finisherID) && finisherID >= 0)
            {
                finishedPlayers.Add(finisherID);
            }
        }
        Debug.Log(finishedPlayers.Count);

        CheckFinished();
    }

    [ServerRpc(RequireOwnership = false)]
    public void GetPlayersServerRpc()
    {
        GetPlayersClientRpc();
    }
    [ClientRpc]
    private void GetPlayersClientRpc()
    {
        onlinePlayers.Clear();

        var items = GameObject.FindGameObjectsWithTag("Player").ToList();
        foreach (var item in items)
        {
            if(item.GetComponent<OnlinePlayer>() != null)
            {
                onlinePlayers.Add(item.GetComponent<OnlinePlayer>());
            }
        }
    }

    [ClientRpc]
    private void SwitchStateClientRpc(int state) {

        if (isFirstSetup)
        {
            Time.timeScale = 1;
            isFirstSetup = false;
        }

        currentState = (State)state;

        if (finishedPlayers == null)
        {
            finishedPlayers = new List<int>();
        }
        finishedPlayers.Clear();

        Debug.Log(currentState.ToString());

        switch (currentState)
        {
            case State.countdownState:
                InitializeCountdownState();
                return;
            case State.parkourState:
                InitializeParkourState();
                return;
            case State.statsState:
                InitializeStatsState();
                return;
            case State.placingState:
                InitializePlacingState();
                return;
        }

    }

    [ClientRpc]
    private void ClearPlacableItemsClientRpc()
    {
        if (!IsServer)
        {
            PlaceableManager.ClearPlacables();
        }
    }    
    
    [ClientRpc]
    private void AddPlacableClientRpc(int id)
    {
        if (!IsServer)
        {
            PlaceableManager.AddSinglePlacable(id);
        }
    }
    [ClientRpc]
    private void SetSelectibleItemsClientRpc() {

        PlacableHud.GetComponent<PlacableObjectsHud>().SetIcons(PlaceableManager.GetPlaceables());
        PlayerInteractionManager.Instance.SetPlacablesHands(PlaceableManager.GetPlaceables());
        //foreach (HandControllerOnline hand in hands)
        //{
        //    hand.SetPlaceableObjectList(PlaceableManager.GetPlaceables());
        //}
    }
    public void RegisterHand(HandControllerOnline hand)
    {
        if (hands.Contains(hand))
        {
            return;
        }
        Debug.Log(hands.Count + " Hands");
        hands.Add(hand);
    }
    public void OnPlayerScore(int pId, int points, float timeTaken)
    {
        OnPlayerScoreServerRpc(pId, points, timeTaken);
    }
    [ServerRpc(RequireOwnership = false)]
    private void OnPlayerScoreServerRpc(int pId, int points, float timeTaken)
    {
        Debug.Log(pId + " " + points);

        OnPlayerScoreClientRpc(pId, points, timeTaken);
    }
    [ClientRpc]
    private void OnPlayerScoreClientRpc(int pId, int points, float timeTaken)
    {
        ScorePlayerCounter++;

        if (playerIdPoints == null)
        {
            playerIdPoints = new Dictionary<int, int>();
        }

        if (playerIdPoints.ContainsKey(pId))
        {
            playerIdPoints[pId] += points;
        }
        else
        {
            playerIdPoints.Add(pId, points);
        }


        if (playerIdPointsThisRound == null)
        {
            playerIdPointsThisRound = new Dictionary<int, int>();
        }

        if (playerIdPointsThisRound.ContainsKey(pId))
        {
            playerIdPointsThisRound[pId] = points;
        }
        else
        {
            playerIdPointsThisRound.Add(pId, points);
        }

        if (playerIdTimeTaken == null)
        {
            playerIdTimeTaken = new Dictionary<int, float>();
        }

        if (playerIdTimeTaken.ContainsKey(pId))
        {
            playerIdTimeTaken[pId] = timeTaken;
        }
        else
        {
            playerIdTimeTaken.Add(pId, timeTaken);
        }
    }

    public int GetPlayerPointsTotal(int pId)
    {
        if(playerIdPoints == null)
        {
            return 0;
        }

        if (playerIdPoints.ContainsKey(pId))
        {
            return playerIdPoints[pId];
        }

        return 0;
    }    
    public int GetPlayerPointsThisRound(int pId)
    {
        if (playerIdPointsThisRound == null)
        {
            return 0;
        }

        if (playerIdPointsThisRound.ContainsKey(pId))
        {
            return playerIdPointsThisRound[pId];
        }

        return 0;
    }    
    public float GetPlayerTimeTaken(int pId)
    {
        if (playerIdTimeTaken == null)
        {
            return 0;
        }

        if (playerIdTimeTaken.ContainsKey(pId))
        {
            return playerIdTimeTaken[pId];
        }

        return 0;
    }
    public void FinishPlayerCamSwap()
    {
        //Debug.Log("Here");

        PlacingCamera.SetActive(true);
        ParkourCamera.SetActive(false);
    }


    public void RequestObjectSpawn(int objectID, Vector3 spawnPosition, Quaternion spawnRotation)
    {
        SpawnObjectServerRpc(objectID, spawnPosition, spawnRotation);
    }


    [ServerRpc(RequireOwnership = false)]
    private void SpawnObjectServerRpc(int objectID, Vector3 spawnPosition, Quaternion spawnRotation)
    {
        objectID = PlaceableManager.GetPlacableIDs()[objectID];
        var item = Instantiate(placedItemPrefab);
        item.GetComponent<NetworkObject>().Spawn();
        item.transform.position = spawnPosition;
        item.transform.rotation = spawnRotation;


        //also set ID

        item.GetComponent<PlacedItemOnline>().SetIDClientRpc(spawnedObjectCounter);
        item.GetComponent<PlacedItemOnline>().InitializeServerRpc(objectID, spawnPosition, spawnRotation);

        spawnedObjectCounter++;
    }

    public void RequestDestroyObject(int objectID)
    {
        if (spawnedObjects.ContainsKey(objectID))
        {
            if (IsHost || IsServer)
            {
                Destroy(spawnedObjects[objectID]);
            }


            spawnedObjects.Remove(objectID);
        }
    }

    public void RegisterSpawnedObject(GameObject gameObject, int placedID)
    {
        spawnedObjects.Add(placedID, gameObject);
    }

    public GameObject GetPlacableObjectOnlineFromID(int id)
    {
        return PlaceableManager.GetPlacableFromID(id);
    }
}
