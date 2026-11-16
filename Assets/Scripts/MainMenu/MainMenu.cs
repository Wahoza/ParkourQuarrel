using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Play()
    {
        Loader.LoadLocal(Loader.Scenes.SelectSceneLocal);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}