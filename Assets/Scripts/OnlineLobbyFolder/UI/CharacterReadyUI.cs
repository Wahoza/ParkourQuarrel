using Unity.Netcode;
using UnityEngine;

public class CharacterReadyUI : MonoBehaviour
{
    void QuitOnline()
    {
        NetworkManager.Singleton.Shutdown();
        Loader.LoadLocal(Loader.Scenes.MainMenu);
    }

    void ReadyOnline(bool ready)
    {
        CharacterSelectReady.Instance.SetPlayerReady(ready);
    }    
}
