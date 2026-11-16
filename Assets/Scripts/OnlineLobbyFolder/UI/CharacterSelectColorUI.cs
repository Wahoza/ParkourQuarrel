using UnityEngine;
using UnityEngine.UI;
public class CharacterSelectColorUI : MonoBehaviour
{
    [SerializeField] int colorID;
    [SerializeField] Image image;
    [SerializeField] GameObject selectedGO;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        image.color = OnlineGameFlowManager.Instance.GetPlayerColor(colorID);
        OnlineGameFlowManager.Instance.OnPlayerDataNetworkListChanged += OnlineGameFlowManager_OnPlayerDataNetworkListChanged;
        UpdateIsSelected();
    }

    private void OnlineGameFlowManager_OnPlayerDataNetworkListChanged(object sender, System.EventArgs e)
    {
        if (selectedGO!=null)
        {
            UpdateIsSelected();
        }
    }

    // Update is called once per frame
    void UpdateIsSelected()
    {
        if (OnlineGameFlowManager.Instance.GetPlayerData().colorId == colorID)
        {
            selectedGO.SetActive(true);
        }
        else 
        { 
            selectedGO.SetActive(false); 
        }
    }

    public void ColorPressedButton()
    {
        OnlineGameFlowManager.Instance.SetPlayerColor(colorID);
    }

    private void OnDestroy()
    {
        OnlineGameFlowManager.Instance.OnPlayerDataNetworkListChanged -= OnlineGameFlowManager_OnPlayerDataNetworkListChanged;
    }
}
