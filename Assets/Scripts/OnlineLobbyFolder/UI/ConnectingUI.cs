using UnityEngine;

public class ConnectingUI : MonoBehaviour
{

    private void Start()
    {
        OnlineGameFlowManager.Instance.OnTryingToJoinGame += Instance_OnTryingToJoinGame;
        OnlineGameFlowManager.Instance.OnFailedToJoinGame += Instance_OnFailedToJoinGame; ;

        Hide();
    }

    private void Instance_OnFailedToJoinGame(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void Instance_OnTryingToJoinGame(object sender, System.EventArgs e)
    {
        Show();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
    void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        OnlineGameFlowManager.Instance.OnTryingToJoinGame -= Instance_OnTryingToJoinGame;
        OnlineGameFlowManager.Instance.OnFailedToJoinGame -= Instance_OnFailedToJoinGame;
    }
}
