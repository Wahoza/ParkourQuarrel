using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public enum Scenes
    {
        MainMenu = 0,
        LobbyScene = 1,
        CharacterSelectSceneOnline = 2,
        GameSceneOnline = 3,
        SelectSceneLocal = 4,
        GameSceneLocal = 5,
        WinScene = 6
    }
    private static Scenes scene;
    // Update is called once per frame
    public static void LoadNetwork (Scenes targetScene)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(), UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    public static void LoadLocal(Scenes targetScene)
    {
        SceneManager.LoadSceneAsync(targetScene.ToString());
    }
    public static int GetSceneID(Scenes targetScene)
    {
        return (int)targetScene;
    }
}
