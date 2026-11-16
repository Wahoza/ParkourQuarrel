using System.Collections.Generic;
using UnityEngine;

public class GoalScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] TimerScript timer;
    [SerializeField] float maxPoints;
    [SerializeField] int minPoints;
    [SerializeField] float timeOffset;
    [SerializeField] List<GameObject> Reached;
    
    AudioManager audioManager;
    int GoalsReached = 0;
    private float pointsPerSecond;
    private int noObjectsDivider;
    private void Start()
    {
        GoalsReached = 0;
        Reached = new List<GameObject>();

        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        pointsPerSecond = GameSettingsManager.Instance.GetPointsPerSecond();
    }
    public void Respawn()
    {
        GoalsReached = 0;


        noObjectsDivider = 1;
        int placedObjectsCount = GameManager.Instance.getPlacedItemCount();

        if (placedObjectsCount == 0)
        {
            noObjectsDivider = GameSettingsManager.Instance.GetNoPlacedDivider();
        }

        Reached.Clear();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<PlayerManager>() != null)
        {
            if (!Reached.Contains(other.gameObject))
            {
                int totalPoints = 0;

                if (GameSettingsManager.Instance.GetGameMaxTime() > 0)
                {
                    float elapsedTime = timer.GetElapsedTimeInSeconds();
                    elapsedTime = elapsedTime > 0 ? elapsedTime : 0;

                    int pointsThisRound = (int)(pointsPerSecond * elapsedTime);
                    if (pointsThisRound < minPoints)
                    {
                        pointsThisRound = minPoints;
                    }

                    totalPoints += pointsThisRound;
                    other.gameObject.GetComponent<PlayerManager>().TimeToFinish(GameSettingsManager.Instance.GetGameMaxTime() - elapsedTime);
                }

                totalPoints += GameSettingsManager.Instance.GetPointsForPlacement() * (PlayerConfigManager.Instance.GetPlayerConfigs().Count - GoalsReached);
                totalPoints /= noObjectsDivider;

                other.gameObject.GetComponent<PlayerManager>().IncreaseScore(totalPoints, GoalsReached);

                GoalsReached++;
                Reached.Add(other.gameObject);

                other.gameObject.GetComponent<PlayerManager>().OnGoalFinished();
                audioManager.PlaySFX(audioManager.goalAudio);
            }
        }
    }
    public int getFinisherCount()
    {
        return GoalsReached;
    }
}
