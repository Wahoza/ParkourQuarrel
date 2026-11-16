using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectPlayer : MonoBehaviour
{
    [SerializeField] private int playerIndex;
    [SerializeField] GameObject readyText;
    [SerializeField] OnlineMaterialChanger matChanger;
    [SerializeField] private Button kickButton;
    private void Start()
    {
        OnlineGameFlowManager.Instance.OnPlayerDataNetworkListChanged += Instance_OnPlayerDataNetworkListChanged;
        CharacterSelectReady.Instance.OnReadyChangend += CharacterSelectReady_OnReadyChangend;

        kickButton.gameObject.SetActive(NetworkManager.Singleton.IsServer && playerIndex != 0);

        UpdatePlayer();
    }

    private void CharacterSelectReady_OnReadyChangend(object sender, System.EventArgs e)
    {
        UpdatePlayer();
    }

    private void Instance_OnPlayerDataNetworkListChanged(object sender, System.EventArgs e)
    {
        UpdatePlayer();
    }
    private void UpdatePlayer()
    {
        if (OnlineGameFlowManager.Instance.IsPlayerIndexConnected(playerIndex))
        {
            Show();

            PlayerData pData = OnlineGameFlowManager.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
            readyText.SetActive(CharacterSelectReady.Instance.isPlayerReady(pData.clientId));

            matChanger.SetPlayerColor(OnlineGameFlowManager.Instance.GetPlayerColor(pData.colorId));
            matChanger.SetFace(OnlineGameFlowManager.Instance.GetPlayerFaceTexture(pData.faceId));
        }
        else
        {
            Hide();
        }

    }    

    void Show()
    {
        gameObject.SetActive(true);
    }

    void Hide() 
    {
        gameObject.SetActive(false); 
    }

    public void KickPlayer()
    {
        Debug.Log("TryKick");
        PlayerData player = OnlineGameFlowManager.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
        OnlineGameFlowManager.Instance.KickPlayer(player.clientId);
    }

    private void OnDestroy()
    {
        OnlineGameFlowManager.Instance.OnPlayerDataNetworkListChanged -= Instance_OnPlayerDataNetworkListChanged;
        CharacterSelectReady.Instance.OnReadyChangend -= CharacterSelectReady_OnReadyChangend;
    }
}
