using System.Linq;
using TMPro;
using UnityEngine;

public class TimerScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private TextMeshProUGUI TimerText;
    [SerializeField] private float bonusTime;
    [SerializeField] private Color BaseColor;
    [SerializeField] private Color DangerColor;
    [SerializeField] private float DangerStartTime;

    private float MaxTimeInSeconds;
    private float startTime;
    public bool isElapsed { get; private set; }

    private string secondsString;
    private string minutesString;
    private bool isStarted = false;

    private int lastDangerSecond = 0;
    private AudioManager audioManager;

    void Start()
    { 
        isStarted = false;
        MaxTimeInSeconds = GameSettingsManager.Instance.GetGameMaxTime();

        if(MaxTimeInSeconds > 0)
        {
            gameObject.SetActive(false);
            return; 
        }

        StartTimer();
    }
    // Update is called once per frame
    public void StartTimer() {

        isElapsed = false;

        if (MaxTimeInSeconds < 0) {
            gameObject.SetActive(false);
            return; 
        }
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();

        startTime = Time.time;
        //TimerText.text = "00:00";

        int seconds = (int)MaxTimeInSeconds % 60;
        secondsString = (seconds).ToString();
        secondsString = (secondsString.Count() == 1 ? "0" : "") + secondsString;

        minutesString = ((MaxTimeInSeconds - seconds) / 60).ToString();
        minutesString = (minutesString.Count() == 1 ? "0" : "") + minutesString;

        isStarted = true;
        isElapsed = false;

        TimerText.text = $"{minutesString}:{secondsString}";
        TimerText.color = BaseColor;

        lastDangerSecond = (int)DangerStartTime;
    }
    public float GetElapsedTimeInSeconds()
    {
        return (MaxTimeInSeconds - (Time.time - startTime));
    }
    void FixedUpdate()
    {
        if(!isStarted) { return; }
        if (startTime + MaxTimeInSeconds + bonusTime <= Time.time)
        {
            isElapsed = true;
            isStarted = false;
            GameManager.Instance.CheckFinished();
        }
        else {

            int elapsedTime = (int)(MaxTimeInSeconds - (Time.time - startTime));
            
            if (elapsedTime >= 0) {
                int seconds = elapsedTime % 60;
                secondsString = (seconds).ToString();
                secondsString = (secondsString.Count() == 1? "0" : "")+ secondsString;

                minutesString = ((elapsedTime - seconds) / 60).ToString();
                minutesString = (minutesString.Count() == 1 ? "0" : "") + minutesString;

                TimerText.text = $"{minutesString}:{secondsString}";
            }
            
            if(elapsedTime < DangerStartTime)
            {
                TimerText.color = DangerColor;

                if((int)elapsedTime < lastDangerSecond)
                {
                    audioManager.PlaySFX(audioManager.countdownBeep);
                    lastDangerSecond = (int)elapsedTime;
                }
            }
        }
    }
}
