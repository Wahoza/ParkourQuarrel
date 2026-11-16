using UnityEngine;

public class LobbyUI : MonoBehaviour
{
    public void BackToMenu()
    {
        Loader.LoadLocal(Loader.Scenes.MainMenu);
    }

    public void CreateLobby()
    {
        PQLobby.Instance.CreateLobby("Lobby", false);
    }

    public void QuickJoin()
    {
        PQLobby.Instance.QuickJoin();
    }
}
