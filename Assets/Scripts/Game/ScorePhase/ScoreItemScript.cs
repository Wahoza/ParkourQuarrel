using System.Collections.Generic;
using System.Linq;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreItemScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] RawImage ColorItem;

    [SerializeField] Image Scorer;
    [SerializeField] Image ScorerOutline;
    [SerializeField] Image ScoreLine;
    [SerializeField] float ScoreLineOffset;

    [SerializeField] RawImage FaceImage;
    [SerializeField] RawImage MedalHolder;
    [SerializeField] RawImage FailHolder;
    [SerializeField] TextMeshProUGUI NameHolder;
    [SerializeField] Transform Goal;
    
    [SerializeField] TextMeshProUGUI PointsText;
    [SerializeField] TextMeshProUGUI TimerText;
    [SerializeField] int PointsRequired;

    int pointsThisRound;
    int points;

    [SerializeField] float AnimDuration;
    float EndTime = 0;


    [SerializeField] List<Texture> Medals;
    public bool isFinished = true;
    private bool allFinished= false;
    //int placement = 0;
    private float scaleFactor;
    PlayerConfig playerConfig;
    public void InitializePlayer(PlayerConfig pc)
    {
        points = 0;
        pointsThisRound = 0;

        Color color = pc.PlayerMaterial.color;
        color.a = 0.8f;
        ColorItem.color = color;
        Scorer.color = color;

        FaceImage.texture = pc.PlayerFace;
        playerConfig = pc;
        transform.localScale = Vector3.one * 0.1f;
        NameHolder.text = "P" + (pc.PlayerIndex +1).ToString();

        Scorer.rectTransform.sizeDelta = new Vector2(1, Scorer.rectTransform.sizeDelta.y);
        ScorerOutline.rectTransform.sizeDelta = new Vector2(1, ScorerOutline.rectTransform.sizeDelta.y);

        ScoreLine.rectTransform.sizeDelta = new Vector2(Goal.localPosition.x + ScoreLineOffset, ScoreLine.rectTransform.sizeDelta.y);
        
        isFinished = true;

        PointsRequired = GameSettingsManager.Instance.GetRequiredPoints();
    }
    public void SetColor(Color color)
    {
        color.a = 0.8f;
        ColorItem.color = color;
        Scorer.color = color;
    }
    public void SetName(string name)
    {
        NameHolder.text = name;
    }
    public void SetFace(Texture2D face)
    {
        FaceImage.texture = face;
    }
    public void StartPhase(bool _allFinished)
    {
        allFinished = _allFinished;
        scaleFactor = PointsRequired != 0 ? (Goal.localPosition.x) / PointsRequired : 1;
        if (allFinished)
        {
            isFinished = true;
            MedalHolder.gameObject.SetActive(false);
            FailHolder.gameObject.SetActive(false);

            GameManager.Instance.AnimScoreboardEnd(1);
            PointsText.gameObject.SetActive(false);
            TimerText.gameObject.SetActive(false);
            return;
        }
        if (pointsThisRound == 0)
        {
            MedalHolder.gameObject.SetActive(false);
            FailHolder.gameObject.SetActive(true);
            PointsText.gameObject.SetActive(false);
            TimerText.gameObject.SetActive(false);
        }
        else
        {
            MedalHolder.gameObject.SetActive(true);
            FailHolder.gameObject.SetActive(false);
            PointsText.gameObject.SetActive(true);

            if (GameSettingsManager.Instance.GetGameMaxTime() < 0)
            {
                TimerText.gameObject.SetActive(false);
            }
            else
            {
                TimerText.gameObject.SetActive(true);
            }

            PointsText.text = "+0";
        }
    }
    public void StartPhaseOnline(bool _allFinished)
    {
        Debug.Log(pointsThisRound + " Points this round Online");
        allFinished = _allFinished;
        scaleFactor = PointsRequired != 0 ? (Goal.localPosition.x) / PointsRequired : 1;

        if (allFinished)
        {
            isFinished = true;
            MedalHolder.gameObject.SetActive(false);
            FailHolder.gameObject.SetActive(false);

            ServerGameManager.Instance.AnimScoreboardEnd(1);
            PointsText.gameObject.SetActive(false);
            TimerText.gameObject.SetActive(false);
            return;
        }
        if (pointsThisRound == 0)
        {
            MedalHolder.gameObject.SetActive(false);
            FailHolder.gameObject.SetActive(true);
            PointsText.gameObject.SetActive(false);
            TimerText.gameObject.SetActive(false);
        }
        else
        {
            MedalHolder.gameObject.SetActive(true);
            FailHolder.gameObject.SetActive(false);
            PointsText.gameObject.SetActive(true);
        }
    }

    public void StartAnim()
    {
        EndTime = Time.time + AnimDuration;
        isFinished = false;
    }
    public void SetPointsThisRound(int pointsGot, int placement)
    {
        pointsThisRound = pointsGot;
        if(placement >= Medals.Count || placement < 0) { return; }
        MedalHolder.texture = Medals[placement];
    }
    public void SetTimeThisRound(float takenTime)
    {
        int time = (int)takenTime;
        string secondsString;
        string minutesString;
        int seconds = time % 60;
        secondsString = (seconds).ToString();
        secondsString = (secondsString.Count() == 1 ? "0" : "") + secondsString;

        minutesString = ((time - seconds) / 60).ToString();
        minutesString = (minutesString.Count() == 1 ? "0" : "") + minutesString;

        TimerText.text = $"{minutesString}:{secondsString}";
    }
    public void FixedUpdate()
    {
        if (!isFinished && !allFinished)
        {

            float t = Mathf.Clamp01((EndTime - Time.time) / AnimDuration + 0.01f);

            float distance = Mathf.Lerp(points + pointsThisRound, points, t);
            //Debug.Log(distance);

            //if (distance >= PointsRequired)
            //{
            //    TriggerWin();
            //}
            PointsText.text = "+"+((int)distance).ToString();
            if (distance >= PointsRequired)
            {
                TriggerWin();

                return;
            }

            
            //Debug.Log(scaleFactor);
            distance *= scaleFactor;


            ScorerOutline.rectTransform.sizeDelta = new Vector2(distance, ScorerOutline.rectTransform.sizeDelta.y);
            Scorer.rectTransform.sizeDelta = new Vector2(distance, Scorer.rectTransform.sizeDelta.y);

            
            if (EndTime < Time.time - 0.01f)
            {

                isFinished = true;
                PointsText.text = "+" + pointsThisRound.ToString();

                points += pointsThisRound;
                pointsThisRound = 0;
                ScorerOutline.rectTransform.sizeDelta = new Vector2(points * scaleFactor, ScorerOutline.rectTransform.sizeDelta.y);
                Scorer.rectTransform.sizeDelta = new Vector2(points * scaleFactor, Scorer.rectTransform.sizeDelta.y);

                if (points >= PointsRequired)
                {
                    TriggerWin();
                }
                else
                {

                    if(ServerGameManager.Instance != null)
                    {
                        ServerGameManager.Instance.AnimScoreboardEnd(1);
                    }
                    else if (GameManager.Instance != null)
                    {
                        GameManager.Instance.AnimScoreboardEnd(1);
                    }

                }
            }
        }

    }

    void TriggerWin()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.EndGame();
        }
        else if (ServerGameManager.Instance != null)
        {
            ServerGameManager.Instance.EndGame();
        }

        isFinished = true;
    }
    
    public void GetFinalPoints()
    {
        if (!isFinished)
        {
            points += pointsThisRound;
            isFinished = true;
        }
        playerConfig.FinalPoints = points;
    }
    public void InitializeOnline(string playerName, Color color, Texture2D face)
    {
        SetName(playerName);
        SetColor(color);
        SetFace(face);

        Scorer.rectTransform.sizeDelta = new Vector2(1, Scorer.rectTransform.sizeDelta.y);
        ScorerOutline.rectTransform.sizeDelta = new Vector2(1, ScorerOutline.rectTransform.sizeDelta.y);

        ScoreLine.rectTransform.sizeDelta = new Vector2(Goal.localPosition.x + ScoreLineOffset, ScoreLine.rectTransform.sizeDelta.y);

        isFinished = true;

        PointsRequired = GameSettingsManager.Instance.GetRequiredPoints();
    }
}
