using UnityEngine;
using TMPro;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.InputSystem;

public class ScoreBoardScript : MonoBehaviour
{
    [SerializeField] float DropDuration;
    [SerializeField] GameObject AButton;
    [SerializeField] GameObject AllFinishedText;
    [SerializeField] GameObject AllFailText;
    [SerializeField] string mapResetString;
    [SerializeField] string allFailString;

    PlayerInputMap playerInputMap;
    bool isDropping = false;
    float DropEndTime;

    bool isReadyForEnd = false;
    bool isFinished = false;
    bool allFinished = false;
    bool gameEnded = false;

    [SerializeField] bool isOnline = false;
    public void StartPhase(bool _allFinished, bool _allFailed, int consecutiveFails)
    {
        allFinished = _allFinished;

        isDropping = true;
        DropEndTime = Time.time + DropDuration;

        isReadyForEnd = false;
        isFinished = false;

        AButton.SetActive(false);

        AllFinishedText.SetActive(allFinished);
        AllFailText.SetActive(false);

        if (_allFailed)
        {
            if (consecutiveFails == 0)
            {
                AllFailText.GetComponent<TextMeshProUGUI>().text = mapResetString;
            }
            else
            {
                AllFailText.GetComponent<TextMeshProUGUI>().text = allFailString + " " + consecutiveFails.ToString();
            }
            AllFailText.SetActive(true);
        }


        if (isOnline) {

            if (ServerGameManager.Instance.IsHost)
            {
                if(playerInputMap == null)
                {
                    playerInputMap = new PlayerInputMap();


                    GetComponent<PlayerInput>().onActionTriggered += Input_onActionTriggered;
                }
                
            }
        }
    }

    public void FixedUpdate()
    {
        if (isDropping)
        {
            if (DropEndTime < Time.time) {

                isDropping = false;

                NotifyEnd();

            }
        }
    }

    public void NotifyEnd()
    {
        if (isOnline) {
            ServerGameManager.Instance.AnimScoreboardEnd(0);
        }
        else
        {
            GameManager.Instance.AnimScoreboardEnd(0);
        }
    }
    public bool GetIsFinished()
    {
        return isFinished;
    }
    public void EnableSwitch()
    {
        if (isOnline)
        {
            if (ServerGameManager.Instance.IsHost)
            {
                isReadyForEnd = true;
                AButton.SetActive(true);
            }
        }
        else
        {
            isReadyForEnd = true;
            AButton.SetActive(true);
        }
    }

    public void SetController(PlayerConfig playerConfig)
    {
        playerInputMap = new PlayerInputMap();
        playerConfig.Input.onActionTriggered += Input_onActionTriggered;
    }

    private void Input_onActionTriggered(CallbackContext obj) {


        if (isReadyForEnd && !gameEnded)
        {
            if(obj.action.name == playerInputMap.Player.Confirm.name)
            {
                if (obj.phase == InputActionPhase.Started)
                {
                    isFinished = true;
                    
                    GameManager.Instance.CheckFinished();
                }
            }
        }
    }
    public void GameEnd()
    {
        gameEnded = true;
    }
}
