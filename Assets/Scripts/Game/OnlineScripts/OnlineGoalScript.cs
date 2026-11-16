using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnlineGoalScript : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] TimerScript timer;
    [SerializeField] int minPoints;
    [SerializeField] float timeOffset;
    
    AudioManager audioManager;
    int GoalsReached = 0;
    private float pointsPerSecond;
    private int noObjectsDivider = 1;


    private bool reached;
    private void Start()
    {
        GoalsReached = 0;

        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        pointsPerSecond = GameSettingsManager.Instance.GetPointsPerSecond();
    }
    public void Respawn()
    {
        GoalsReached = 0;

        reached = false;

        noObjectsDivider = 1;
        int placedObjectsCount = ServerGameManager.Instance.getPlacedItemCount();

        if (placedObjectsCount == 0)
        {
            noObjectsDivider = GameSettingsManager.Instance.GetNoPlacedDivider();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == PlayerInteractionManager.Instance.GetLocalPlayer())
        {
            if (!reached)
            {
                int totalPoints = 0;
                float elapsedTime = -1;

                if (GameSettingsManager.Instance.GetGameMaxTime() > 0)
                {
                    elapsedTime = timer.GetElapsedTimeInSeconds();
                    elapsedTime = elapsedTime > 0 ? elapsedTime : 0;

                    int pointsThisRound = (int)(pointsPerSecond * elapsedTime);
                    if (pointsThisRound < minPoints)
                    {
                        pointsThisRound = minPoints;
                    }

                    totalPoints += pointsThisRound;
                    other.gameObject.GetComponent<PlayerManager>().TimeToFinish(GameSettingsManager.Instance.GetGameMaxTime() - elapsedTime);
                }

                totalPoints += GameSettingsManager.Instance.GetPointsForPlacement() * (OnlineGameFlowManager.Instance.GetPlayerCount() - GoalsReached);
                totalPoints /= noObjectsDivider;

                ServerGameManager.Instance.OnPlayerScore(PlayerInteractionManager.Instance.GetPlayerId(), totalPoints, elapsedTime);
                Debug.Log("I Scored!");
                reached = true;


                other.gameObject.GetComponent<PlayerManager>().OnGoalFinished();
                //other.gameObject.GetComponent<OnlinePlayer>().FinishInstance();


                PlayerInteractionManager.Instance.DespawnPlayer();

                audioManager.PlaySFX(audioManager.goalAudio);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void IncreseFinisherCountServerRpc() { 
        IncreaseFinisherCountClientRpc();
    }
    [ClientRpc]
    private void IncreaseFinisherCountClientRpc()
    {
        GoalsReached++;
        Debug.Log(GoalsReached);
    }
    public int getFinisherCount()
    {
        return GoalsReached;
    }
}
