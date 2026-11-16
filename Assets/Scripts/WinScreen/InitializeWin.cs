using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class InitializeWin : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] GameObject playerPrefab;
    [SerializeField] List<Transform> playerSpawns;
    [SerializeField] Camera mainCamera;
    [SerializeField] GameObject tears;
    [SerializeField] TextMeshProUGUI winTextField;
    [SerializeField] string winText;
    [SerializeField] WinScreenOptions OptionsField;
    private List<GameObject> players = new List<GameObject>();
    PlayerConfig[] playerConfigs;
    void Start()
    {
        Cursor.visible = false;

        playerConfigs = PlayerConfigManager.Instance.GetPlayerConfigs().ToArray();

        OptionsField.Initialize(playerConfigs[0]); //p0 has controlls

        Array.Sort(playerConfigs,
        delegate (PlayerConfig x, PlayerConfig y) { return x.FinalPoints.CompareTo(y.FinalPoints); });

        Array.Reverse(playerConfigs);

        playerSpawns[playerConfigs.Count() - 1].transform.rotation *= Quaternion.Euler(90, 10, 0);
        playerSpawns[playerConfigs.Count() - 1].transform.position += Vector3.up * 0.1f;

        for (int i = 0; i < playerConfigs.Count(); i++)
        {
            var player = Instantiate(playerPrefab, playerSpawns[i].position, playerSpawns[i].rotation, gameObject.transform);

            player.transform.GetChild(0)?.GetComponent<PlayerInputHandler>().InitializePlayer(playerConfigs[i]);
            player.transform.GetChild(0)?.GetComponent<CircleSync>().SetCamera(mainCamera);
            player.transform.GetChild(0)?.GetComponent<CircleSync>().SetPlayerID(playerConfigs[i].PlayerIndex);
            player.transform.GetChild(0)?.GetComponent<GrassSyncScrpit>().SetPlayerID(playerConfigs[i].PlayerIndex);

            player.transform.GetChild(0)?.gameObject.GetComponent<PlayerManager>().Respawn();


            PlayerController pc = player.transform.GetChild(0)?.gameObject.GetComponent<PlayerController>();
            //pc.Respawn();
            pc.enabled = false;

            player.transform.GetChild(1)?.gameObject.SetActive(false);

            players.Add(player.transform.GetChild(0).gameObject);
        }

        for (int i = playerSpawns.Count - 1; i >= playerConfigs.Count(); i--) {
            playerSpawns[i].gameObject.SetActive(false);
        }
        winTextField.text = "P"+(playerConfigs[0].PlayerIndex + 1).ToString() + winText;
        //Debug.Log(players[playerConfigs.Count() - 1].transform.rotation);
        tears.transform.position = playerSpawns[playerConfigs.Length - 1].position;
    }

    // Update is called once per frame
    void Update()
    {
        //players[playerConfigs.Count() - 1].transform.rotation = Quaternion.Euler(90, 0, 0);
    }
}
