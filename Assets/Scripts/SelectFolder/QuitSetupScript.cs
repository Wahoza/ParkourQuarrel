using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuitSetupScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private bool startedQuit;
    [SerializeField] float quitTime;
    [SerializeField] float appearTime; //for alpha of the quitIco
    [SerializeField] Image QuitIcon;
    [SerializeField] Color QuitColor; //alpha should be 0

    [SerializeField] Image QuitCircle;
    [SerializeField] Image QuitBG;
    [SerializeField] float maxQuitBGAplha;

    private float quitDuration; //same as alphaMulti
    private float alphaMultiplier; //how many Seconds till fill, but inverted to 1/Time

    

    void Start()
    {
        startedQuit = false;
        alphaMultiplier = 1 / appearTime;
        quitDuration = 1 / quitTime;
        QuitIcon.color = QuitColor;
        QuitBG.color = new Color(QuitBG.color.r, QuitBG.color.g, QuitBG.color.b, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            startedQuit = true;

        }

        if (Input.GetButtonUp("Cancel"))
        {
            startedQuit = false;
            Color baseColor = new Color(1f,1f,1f,0f);

            QuitIcon.color = QuitColor;
            QuitCircle.fillAmount = 0;

            QuitBG.color = new Color(QuitBG.color.r, QuitBG.color.g, QuitBG.color.b, 0);
        }

        if (startedQuit)
        {
            if (QuitIcon.color.a < 1)
            {
                QuitIcon.color = new Color(QuitColor.r, QuitColor.g, QuitColor.b, QuitIcon.color.a + alphaMultiplier * Time.deltaTime);
            }
            if (QuitBG.color.a < maxQuitBGAplha) 
            {
                QuitBG.color = new Color(QuitBG.color.r, QuitBG.color.g, QuitBG.color.b, QuitBG.color.a + alphaMultiplier * Time.deltaTime);
            }


            QuitCircle.fillAmount += 1 / quitTime * Time.deltaTime;

            if (QuitCircle.fillAmount >= 1)
            {
                PlayerConfigManager.Instance.ClearPlayers();
                Loader.LoadLocal(Loader.Scenes.MainMenu);
            }
        }
    }
}
