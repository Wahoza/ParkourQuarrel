using UnityEngine;
using UnityEngine.UI;
public class CharacterSelectFaceUI : MonoBehaviour
{
    [SerializeField] int faceId;
    [SerializeField] RawImage faceSprite;
    [SerializeField] GameObject selectedGO;

    private Texture2D face;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Texture2D faceTexture = OnlineGameFlowManager.Instance.GetPlayerFaceTexture(faceId);

        faceSprite.texture = faceTexture;

        OnlineGameFlowManager.Instance.OnPlayerDataNetworkListChanged += OnlineGameFlowManager_OnPlayerDataNetworkListChanged;
        UpdateIsSelected();
    }

    private void OnlineGameFlowManager_OnPlayerDataNetworkListChanged(object sender, System.EventArgs e)
    {
        UpdateIsSelected();
    }

    // Update is called once per frame
    void UpdateIsSelected()
    {
        if (OnlineGameFlowManager.Instance.GetPlayerData().faceId == faceId)
        {
            selectedGO.SetActive(true);
        }
        else
        {
            selectedGO.SetActive(false);
        }
    }

    public void FacePressedButton()
    {
        OnlineGameFlowManager.Instance.SetPlayerFace(faceId);
    }
}
