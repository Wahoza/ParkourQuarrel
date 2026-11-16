using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class MainMenuCleanUp : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if(NetworkManager.Singleton != null)
        {
            Destroy(NetworkManager.Singleton.gameObject);
        }


        if(PlayerInteractionManager.Instance != null)
        {
            Destroy(PlayerInteractionManager.Instance.gameObject);
        }
        if(OnlineGameFlowManager.Instance != null)
        {
            Destroy(OnlineGameFlowManager.Instance.gameObject);
        }

        if (GameSettingsManager.Instance != null)
        {
            Destroy(GameSettingsManager.Instance.gameObject);
        }
        if (PlayerConfigManager.Instance != null)
        {
            Destroy(PlayerConfigManager.Instance.gameObject);
        }
        if (PQLobby.Instance != null)
        {
            Destroy(PQLobby.Instance.gameObject);
        }
    }
}
