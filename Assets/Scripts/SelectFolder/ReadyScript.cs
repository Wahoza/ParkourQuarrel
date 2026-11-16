using UnityEngine;

public class ReadyScript : MonoBehaviour
{
    [SerializeField] private GameObject SetupMenu;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void CancelReady() {

        GameObject.FindGameObjectWithTag("GameSettingsWindow").GetComponent<GameSettings>().Cancel();
                
        SetupMenu.GetComponent<PlayerSetupMenuController>().enabled = true;
        SetupMenu.transform.GetChild(0).gameObject.SetActive(true);
        SetupMenu.GetComponent<PlayerSetupMenuController>()?.UnReadyPlayer();
        gameObject.SetActive(false);
    }
}
