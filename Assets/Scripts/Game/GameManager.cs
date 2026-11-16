using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
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
    List<HandController> hands;
    List<PlayerManager> playerManagers;
    List<GameObject> placedItems;

    [Header("ParkourStuff")]
    [SerializeField] GameObject ParkourCamera;
    [SerializeField] GameObject ParkourHud;
    [SerializeField] GameObject Goal;
    [SerializeField] TimerScript Timer;

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

    public static GameManager Instance { get; private set; }

    public void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("Trying to create new instance");
        }
        else
        {
            Instance = this;
            //DontDestroyOnLoad(Instance);
            players = new List<GameObject>();
            placedItems = new List<GameObject>();
            playerManagers = new List<PlayerManager>();
            hands = new List<HandController>();
            scoreScripts = new List<ScoreItemScript>();

            consecutiveFails = 0;
            allFinished = false;
        }
    }
    public virtual void Start()
    {
        InitializeCountdownState();
    }

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

        foreach (GameObject player in players)
        {
            //player.SetActive(true);
            //player.GetComponent<PlayerController>().enabled = true;
            player.GetComponent<PlayerManager>().Respawn();
            player.GetComponent<PlayerController>().CanMove(true);

        }

        foreach (GameObject placedItem in placedItems)
        {
            placedItem.GetComponent<PlacedItem>().StartParkourState();
        }

        foreach (GameObject placedItem in InitialPlacedItems)
        {
            placedItem.GetComponent<PlacedItem>().StartParkourState();
        }

        Goal.GetComponent<GoalScript>()?.Respawn();
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

        int finishedCount = Goal.GetComponent<GoalScript>().getFinisherCount();

        allFinished = finishedCount == players.Count && players.Count > maxFinishPlayers;

        if (finishedCount == 0)
        {
            consecutiveFails++;
        }
        else
        {
            consecutiveFails = 0;
        }

        foreach (GameObject player in players)
        {
            player.GetComponent<PlayerController>().CanMove(false);
        }

        ScoreBoard.StartPhase(allFinished, finishedCount == 0, maxConsecutiveFails - consecutiveFails + 1); //+1 because of string readability

        if (consecutiveFails > maxConsecutiveFails) { ResetBoard(); }
    }
    void InitializePlacingState()
    {
        foreach (GameObject player in players)
        {
            player.GetComponent<PlayerManager>().EnableGrave(false);
        }

        foreach (PlayerManager playerManager in playerManagers)
        {
            playerManager.DisableGameObject();
        }

        ParkourCamera.SetActive(false);
        PlacingCamera.SetActive(true);
        ParkourHud.SetActive(false);
        ScoreHud.DisableScoreBoard();
        PlacingHud.SetActive(true);
        PlacableHud.SetActive(true);
        Timer.gameObject.SetActive(false);
        CountdownHud.SetActive(false);

        PlaceableManager.GenerateRandomPlacables(placableCount);

        PlacableHud.GetComponent<PlacableObjectsHud>().SetIcons(PlaceableManager.GetPlaceables());

        int handID = -hands.Count/2;
        foreach(HandController hand in hands)
        {
            handID++;
            
            hand.SetPlaceableObjectList(PlaceableManager.GetPlaceables());
            hand.StartPlacing(handID);
        }

        foreach(GameObject placedItem in placedItems)
        {
            placedItem.GetComponent<PlacedItem>().StartPlaceState();
        }
        foreach (GameObject placedItem in InitialPlacedItems)
        {
            placedItem.GetComponent<PlacedItem>().StartPlaceState();
        }
        foreach(GameObject player in players)
        {
            //note there is an issue with respawning during the countdown, idk what it is though, so we just reset the player controller here and reset the Manager in the Init Parkour
            player.GetComponent<PlayerController>().Respawn();
            player.GetComponent<PlayerController>().CanMove(false);
        }
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
        
        //initCam.SetActive(false);

        foreach (GameObject player in players)
        {
            player.GetComponent<PlayerManager>().EnableGameObject();
            player.GetComponent<PlayerManager>().FocusCam();

            //resets position for nice visual, im not the biggest fan of doing it like this
            player.GetComponent<PlayerController>().Respawn();
            player.GetComponent<PlayerController>().CanMove(false);

            //player.GetComponent<PlayerController>().enabled = false;
        }
    }
    public virtual void CheckFinished()
    {
        switch (currentState)
        {
            case State.parkourState:
                if (playerManagers.All(p => (int)p.currentState != 0) || Timer.isElapsed) //implies that the player is dead or won
                {
                    currentState = State.statsState;

                    InitializeStatsState();
                }
                return;
            case State.statsState:
                if (ScoreBoard.GetIsFinished())
                {
                    currentState = State.placingState;

                    InitializePlacingState();
                }
            return;
            case State.placingState:
                if (hands.All(h => h.GetCurrentState() >= 2))
                {
                    currentState = State.countdownState;

                    InitializeCountdownState();
                }
            return;
            case State.countdownState:
                if (countdownScript.state == CountdownScript.currentState.finished) 
                {
                    currentState = State.parkourState;

                    InitializeParkourState();
                }
            return;
        }
    }


    public void AddPlayer(GameObject go)
    {
        if (go == null) { return; }

        if (players.Contains(go)) { return; }
        players.Add(go);
        playerManagers.Add(go.GetComponent<PlayerManager>());
        
        if(currentState != State.parkourState)
        {
            go.GetComponent<PlayerController>().CanMove(false);
            //go.GetComponent<PlayerStats>().Respawn();

            //go.GetComponent<PlayerStats>().Respawn();
        }
    }


    public void AddHand(GameObject go)
    {
        if (go.GetComponent<HandController>() == null) { return; }
        if (hands.Contains(go.GetComponent<HandController>())) { return; }

        hands.Add(go.GetComponent<HandController>());
    }


    public void AddPlacedItem(GameObject go)
    {
        if (placedItems.Contains(go)) { return; }

        placedItems.Add(go);
    }

    public void RemovePlacedItem(GameObject go)
    {
        if (!placedItems.Contains(go)) { return; }
        placedItems.Remove(go);
    }

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
            foreach (ScoreItemScript script in scoreScripts)
            {
                Debug.Log(allFinished);
                script.StartPhase(allFinished);
                script.StartAnim();

            }
        }
        else
        {
            ScoreBoard.EnableSwitch();
        }
    }

    private void ResetBoard()
    {
        consecutiveFails = 0;
        Debug.Log(placedItems.Count);
        for (int i = placedItems.Count - 1; i >= 0; i--)
        {
            var item = placedItems[i];
            placedItems.Remove(item);
            item.GetComponent<PlacedItem>().StartPlaceState();
            Destroy(item);
        }
    }

    public virtual int getPlacedItemCount() {
        return placedItems.Count();
    }
}
