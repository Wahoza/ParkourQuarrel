using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class PlayerConfigManager : MonoBehaviour
{
    private List<PlayerConfig> playerConfigs; //THESE ALWAYS MUST HAVE SAME INDEX FOR SAME OBJECT.
    private Dictionary<int,Transform> playerTransforms;
    private List<PlayerSetupMenuController> PlayerSetupMenues;
    [SerializeField] private int MaxPlayers;
    [SerializeField] private List<int> takenColors;
    private bool allReady;
    public static PlayerConfigManager Instance {get; private set;}
    
    private void Awake()
    {
        if(Instance == null)

        {
            Instance = this;
            DontDestroyOnLoad(Instance);
            playerConfigs = new List<PlayerConfig>();
            playerTransforms = new Dictionary<int, Transform>();
            PlayerSetupMenues = new List<PlayerSetupMenuController>();
            takenColors = new List<int>();
            allReady = false;
        }
    }
    public List<PlayerConfig> GetPlayerConfigs()
    {
        return playerConfigs;
    }
    public void SetPlayerColor(int index, Material Color)
    {
        playerConfigs[index].PlayerMaterial = Color;
    }
    public void SetPlayerFace(int index, Texture2D Face)
    {
        playerConfigs[index].PlayerFace = Face;
    }

    public void ReadyPlayer(int index)
    {
        playerConfigs[index].isReady = true;
        if(playerConfigs.All(p => p.isReady == true))
        {
            allReady = true;
            GameObject.FindGameObjectWithTag("GameSettingsWindow").GetComponent<GameSettings>().StartSettingsPhase();
        }
    }
    public void TakeColor(int color)
    {
        if (!takenColors.Contains(color))
        {
            takenColors.Add(color);
        }

        foreach (PlayerSetupMenuController controller in PlayerSetupMenues)
        {
            controller.SetColorsTaken(takenColors);
        }
    }
    public void RemoveColor(int color)
    {
        if (takenColors.Contains(color))
        {
            takenColors.Remove(color);
        }
        foreach (PlayerSetupMenuController controller in PlayerSetupMenues)
        {
            controller.SetColorsTaken(takenColors);
        }
    }
    public void UnReadyPlayer(int index)
    {
        playerConfigs[index].isReady = false;
        allReady = false;
    }
    public bool GetAllReady()
    {
        return allReady;
    }
    public void HandlePlayerJoin(PlayerInput pi)
    {        
        if (true)//!playerConfigs.Any(p => p.PlayerIndex == pi.playerIndex))
        {
            pi.transform.SetParent(transform);
            int pID = playerConfigs.Count();
            playerTransforms.Add(pID, pi.transform);
            playerConfigs.Add(new PlayerConfig(pi, pID));
        }
    }

    public void JoinSetupMenu(PlayerSetupMenuController controller)
    {
        
            controller.SetPlayerIndex(PlayerSetupMenues.Count);
            controller.SetColorsTaken(takenColors);

            PlayerSetupMenues.Add(controller);
        
        if (allReady) { controller.Disconnect(); }
    }
    public void LeaveSetupMenu(PlayerSetupMenuController controller, int playerIndex)
    {
        if (PlayerSetupMenues.Contains(controller))
        {
            PlayerSetupMenues.Remove(controller);
        }

        PlayerConfig pc = playerConfigs.FirstOrDefault(i => i.PlayerIndex == playerIndex);

        if (pc != null)
        {
            Transform t = playerTransforms[pc.PlayerIndex];

            playerConfigs.Remove(pc);

            Destroy(t.gameObject);
            playerTransforms.Remove(pc.PlayerIndex);
        }



        //recalc old playerConfigs
        for (int i = playerIndex; i < PlayerSetupMenues.Count; i++)
        {
            PlayerSetupMenues[i].SetPlayerIndex(i);
            playerConfigs[i].PlayerIndex = i;
        }
        Dictionary<int,Transform> newDict = new Dictionary<int,Transform>();

        foreach(KeyValuePair<int,Transform> kvp in playerTransforms)
        {
            int newKey = kvp.Key;

            if(kvp.Key >= playerIndex)
            {
                newKey--;
            }

            newDict.Add(newKey, kvp.Value);
        }

        playerTransforms = newDict;
    }
    public void ResetPlayerConfigPoints()
    {
        foreach (PlayerConfig playerConfig in playerConfigs)
        {
            playerConfig.FinalPoints = 0;
        }
    }
    public void ClearPlayers() 
    {
        playerConfigs.Clear();
        playerTransforms.Clear();
        PlayerSetupMenues.Clear();

        takenColors.Clear();

        allReady = false;

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
    public void UnReadyAll()
    {
        foreach (PlayerSetupMenuController controller in PlayerSetupMenues)
        {
            controller.UnReadyPlayer();
        }
        allReady = false;
    }
}
public class PlayerConfig 
{
    public PlayerConfig(PlayerInput input, int playerIndex)
    {
        Input = input;
        PlayerIndex = playerIndex;
        FinalPoints = 0;
        FinalRoundTime = 0; 
    }

    public PlayerInput Input { get; set; }
    public int PlayerIndex { get; set; }
    public bool isReady { get; set; }
    public bool isPlaying { get; set; }
    public Material PlayerMaterial { get; set; }
    public Texture2D PlayerFace { get; set; }
    public int FinalPoints { get; set; }
    public int FinalRoundTime { get; set; }
}
