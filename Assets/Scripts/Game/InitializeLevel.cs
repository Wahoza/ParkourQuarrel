using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class InitializeLevel : MonoBehaviour
{
    [SerializeField] Transform mainUI;
    [SerializeField] Transform pointerUI;
    [SerializeField] Transform placePhaseUI;
    [SerializeField] RectTransform placePhaseObjects;
    [SerializeField] Transform scorePhaseUIHolder;
    [SerializeField] ScoreBoardScript scoreBoard;

    [SerializeField]
    private List<Transform> playerSpawns;
    [SerializeField]
    private GameObject playerPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] GameObject playerUIStatsPrefab;
    [SerializeField] GameObject playerPointerPrefab;
    [SerializeField] GameObject playerHandPrefab;
    [SerializeField] GameObject playerScorePrefab;
    [SerializeField] private CinemachineTargetGroup cameraTargetGroup;
    [SerializeField] private Camera mainCamera;
    private void Start()
    {
        Cursor.visible = false;

        var playerConfigs = PlayerConfigManager.Instance.GetPlayerConfigs().ToArray();

        for (int i = 0; i < playerConfigs.Length; i++)
        {
            var player = Instantiate(playerPrefab, playerSpawns[i].position, playerSpawns[i].rotation, gameObject.transform);
            player.transform.GetChild(0)?.GetComponent<PlayerInputHandler>().InitializePlayer(playerConfigs[i]);
            player.transform.GetChild(0)?.GetComponent<CircleSync>().SetCamera(mainCamera);
            player.transform.GetChild(0)?.GetComponent<CircleSync>().SetPlayerID(playerConfigs[i].PlayerIndex);
            player.transform.GetChild(0)?.GetComponent<GrassSyncScrpit>().SetPlayerID(playerConfigs[i].PlayerIndex);


            player.transform.GetChild(0)?.GetComponentInChildren<MaterialChanger>().SetFromPlayerConfig(playerConfigs[i]);
            //cameraTargetGroup.AddMember(player.transform.GetChild(0), 1, 5);

            GameManager.Instance.AddPlayer(player.transform.GetChild(0)?.gameObject);

            var playerPointer = Instantiate(playerPointerPrefab);
            playerPointer.GetComponent<PointerScript>()?.InitializePlayer(playerConfigs[i],player.transform.GetChild(0), mainCamera);
            playerPointer.transform.SetParent(pointerUI, true);
            player.transform.GetChild(0)?.GetComponent<PlayerInputHandler>().SetSpeechScript(playerPointer.GetComponent<SpeechScript>());


            //playerManager
            var playerManager = Instantiate(playerUIStatsPrefab);

            playerManager.GetComponent<UiStatsScript>()?.InitializePlayer(playerConfigs[i]);
            playerManager.transform.SetParent(mainUI, false);



            player.transform.GetChild(0)?.GetComponent<PlayerManager>().SetUIStatItem(playerManager.GetComponent<UiStatsScript>());
            player.transform.GetChild(0)?.GetComponent<PlayerManager>().SetCinemaTargetGroup(cameraTargetGroup);
            player.transform.GetChild(0)?.GetComponent<PlayerManager>().SetPlayerPointer(playerPointer);

            //Now initializing Placing phase stuff

            var hand = Instantiate(playerHandPrefab);

            hand.GetComponent<HandleHandInputs>()?.InitializePlayer(playerConfigs[i], placePhaseObjects);
            hand.transform.SetParent(placePhaseUI, false);
            hand.GetComponent<HandController>().HudCamera = mainCamera;

            GameManager.Instance.AddHand(hand);

            //score Stuff
            var playerScore = Instantiate(playerScorePrefab);
            playerScore.transform.SetParent(scorePhaseUIHolder, false);

            playerScore.GetComponent<ScoreItemScript>().InitializePlayer(playerConfigs[i]);

            GameManager.Instance.AddScoreItem(playerScore.GetComponent<ScoreItemScript>());
            player.transform.GetChild(0)?.GetComponent<PlayerManager>().SetScoreItem(playerScore.GetComponent<ScoreItemScript>());


        }

        PauseScript pauseScript = GameObject.FindGameObjectWithTag("PauseMenu").GetComponent<PauseScript>();

        if(pauseScript != null)
        {
            pauseScript.SetPlayerOneConfig(playerConfigs[0]);
        }

        scoreBoard.SetController(playerConfigs[0]);
        //placePhaseUI.gameObject.SetActive(false);
    }

}
