using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ServerGameSettings : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] GameSettings gameSettings;
    [SerializeField] GameObject OptionsButton;
    [SerializeField] EventSystem eventSystem;
    void Start()
    {
        if (!NetworkManager.Singleton.IsHost)
        {
            Hide();
            OptionsButton.SetActive(false);
        }
        else
        {
            gameSettings.FinishedSetupEvent += GameSettings_FinishedSetupEvent;
            OptionsButton.SetActive(true);
        }
    }

    private void GameSettings_FinishedSetupEvent(object sender, System.EventArgs e)
    {

        if (NetworkManager.Singleton.IsHost)
        {
            OptionsButton.SetActive(true);
            eventSystem.enabled = true;
            eventSystem.SetSelectedGameObject(OptionsButton);
        }
    }
    [ServerRpc(RequireOwnership = false)]
    void SetSettingsServerRpc(int time, int pointsPerSecond, int placementPoints, int requiredPoints)
    {
        SetSettingsClientRpc(time, pointsPerSecond, placementPoints, requiredPoints);
    }
    [ClientRpc]
    void SetSettingsClientRpc(int time, int pointsPerSecond, int placementPoints, int requiredPoints) {
        GameSettingsManager.Instance.SetGameTime(time);
        GameSettingsManager.Instance.SetPointsPerSecond(pointsPerSecond);
        GameSettingsManager.Instance.SetPointsForPlacement(placementPoints);
        GameSettingsManager.Instance.SetRequiredPoints(requiredPoints);
    }

    public void ConfirmSettings()
    {
        gameSettings.LoadDefaultSettings();

        int time = GameSettingsManager.Instance.GetGameMaxTime();
        int pointsPerSecond = GameSettingsManager.Instance.GetPointsPerSecond();
        int placementPoints = GameSettingsManager.Instance.GetPointsForPlacement();
        int requiredPoints = GameSettingsManager.Instance.GetRequiredPoints();

        SetSettingsServerRpc(time, pointsPerSecond, placementPoints, requiredPoints);
    }
    // Update is called once per frame
    private void Hide()
    {
        gameObject.SetActive(false);
    }    
    private void Show()
    {
        gameObject.SetActive(true);
    }
}
