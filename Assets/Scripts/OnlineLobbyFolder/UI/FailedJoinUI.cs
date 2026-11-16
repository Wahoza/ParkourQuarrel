using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FailedJoinUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private GameObject closeButton;
    [SerializeField] private EventSystem eventSystem;
    private void Start()
    {
        OnlineGameFlowManager.Instance.OnFailedToJoinGame += Instance_OnFailedToJoinGame; ;

        Hide();
    }

    private void Instance_OnFailedToJoinGame(object sender, System.EventArgs e)
    {
        Show();

        messageText.text = NetworkManager.Singleton.DisconnectReason;

        if(messageText.text == "")
        {
            messageText.text = "Failed to connect";
        }

        eventSystem.SetSelectedGameObject(closeButton);
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
        OnlineGameFlowManager.Instance.OnFailedToJoinGame -= Instance_OnFailedToJoinGame; ;

    }
}
