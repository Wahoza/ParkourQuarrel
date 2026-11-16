using Unity.Netcode;
using UnityEngine;

public class TestingLobbyUI : MonoBehaviour
{
    public void StartHost()
    {
        OnlineGameFlowManager.Instance.StartHost();
        Loader.LoadNetwork(Loader.Scenes.CharacterSelectSceneOnline);
        Hide();
    }
    public void StartClient()
    {
        OnlineGameFlowManager.Instance.StartClient();
        Debug.Log("Starterd Client");
        Hide();
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
