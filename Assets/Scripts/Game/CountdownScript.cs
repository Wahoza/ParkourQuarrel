using UnityEngine;
using TMPro;
using System;
public class CountdownScript : MonoBehaviour
{
    [SerializeField] TMP_Text m_Text;
    [SerializeField] int countdownStart;
    [SerializeField] float camswapNr;
    [SerializeField] int countdownEnd;
    [SerializeField] float speed;

    [SerializeField] GameObject initCam;
    [SerializeField] GameObject parkourCam;

    public event EventHandler FinishedEvent;
    public bool isFirstCountdown = true;
    public enum currentState
    {
        running,
        finished
    }

    public currentState state = currentState.finished;
    int currentNr;
    float currentFloat;
    float startTime;

    AudioManager audioManager;
    
    public void StartCountdown()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        currentNr = countdownStart;
        currentFloat = countdownStart;

        state = currentState.running;
        startTime = Time.time;

        m_Text.text = currentNr.ToString();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (state == currentState.running) {
            currentFloat = (float)countdownStart - (Time.time - startTime) * speed;
            if((int)(currentFloat + 1) < currentNr)
            {
                currentNr = (int) (currentFloat + 1);
                m_Text.text = currentNr.ToString();

                if(isFirstCountdown && currentFloat < camswapNr)
                {
                    initCam.SetActive(false);
                    parkourCam.SetActive(true);

                    isFirstCountdown = false;
                }
                if (currentNr > countdownEnd)
                {
                    audioManager.PlaySFX(audioManager.countdownBeep);
                }
                else
                {
                    audioManager.PlaySFX(audioManager.countdownFinalBeep);
                    state = currentState.finished;

                    GameManager.Instance.CheckFinished();
                    //FinishedEvent.Invoke(this, EventArgs.Empty);
                    //else if(ServerGameManager.Instance != null)
                    //{
                    //    ServerGameManager.Instance.CheckFinished();
                    //}
                    gameObject.SetActive(false);
                }
            }
        }
    }


}
