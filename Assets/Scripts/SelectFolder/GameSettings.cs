using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.UIElements;
using System;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class GameSettings : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] GameObject OptionField;
    [SerializeField] Loader.Scenes SceneToLoad;

    public event EventHandler FinishedSetupEvent;
    
    [Header("Timer")]
    
    
    [SerializeField] TextMeshProUGUI TimerText;
    [SerializeField] GameObject SecondsText;
    [SerializeField] private int maxTime;
    [SerializeField] private int minTime;

    [SerializeField] private int timeStep;

    [SerializeField] private int timeStartValue;

    [SerializeField] string noTimeString;
    [SerializeField] private float InvertedTimerMaxSpeed;
    private int currentTime;

    [Header("TimerPoints")]    
    [SerializeField] TextMeshProUGUI TimerPointsText;
    [SerializeField] private int maxTimePoints;
    [SerializeField] private int minTimePoints;

    [SerializeField] private int timePointsStep;

    [SerializeField] private int timePointsStartValue;
    [SerializeField] private float InvertedTimePointsMaxSpeed;
    private int currentTimePoints;


    [Header("Points")]
    [SerializeField] TextMeshProUGUI PointsText;

    [SerializeField] private int maxPoints;
    [SerializeField] private int minPoints;

    [SerializeField] private int pointsStep;

    [SerializeField] private int pointsStartValue;
    [SerializeField] private float InvertedPointsMaxSpeed;

    private int currentPoints;

    [Header("ReqPoints")]
    [SerializeField] TextMeshProUGUI ReqPointsText;

    [SerializeField] private int maxReqPoints;
    [SerializeField] private int minReqPoints;

    [SerializeField] private int reqPointsStep;

    [SerializeField] private int reqPointsStartValue;
    [SerializeField] private float InvertedReqPointsMaxSpeed;

    private int currentReqPoints;

    [Header("Other")]
    [SerializeField] GameObject AToJoinText;
    [SerializeField] GameObject WarningField;
    [SerializeField] List<GameObject> SelectArrows;

    private int currentSelectedField = 0;
    [SerializeField] private int optionCount;

    //i use "inverted" to symbolise that the speed will not be 1/speed but the raw value will be used.
    [SerializeField] private float InvertedMaxSpeed;
    [SerializeField] private float InvertedStartSpeed;
    [SerializeField] private float SpeedIncrease;
    [SerializeField] LoadingScript loadingScript;
    [SerializeField] Animator settingsAnim;
    private float invertedCurrentSpeed;

    PlayerInputMap controller;
    private PlayerConfig lastPlayer = null;

    private Vector2 directionInput;

    private bool settingsStarted = false;
    private float TimeInPhase = 0;

    [SerializeField] int GameScene;

    // Update is called once per frame
    public void Start()
    {
        settingsStarted = false ;

        ResetSettings();
    }
    
    public void DoSettingsUpdate()
    {
        TimeInPhase += Time.deltaTime;
        if(TimeInPhase > invertedCurrentSpeed)
        {
            switch (currentSelectedField)
            {
                case 0:
                    currentTime += (int)(timeStep * directionInput.x);
                    currentTime = currentTime < minTime - timeStep ? minTime - timeStep : (currentTime > maxTime ? maxTime : currentTime);

                    if (invertedCurrentSpeed > InvertedTimerMaxSpeed && directionInput.x != 0)
                    {
                        invertedCurrentSpeed *= SpeedIncrease;
                    }

                    if (currentTime < minTime)
                    {
                        TimerText.text = noTimeString;
                        SecondsText.SetActive(false);
                    }
                    else
                    {
                        SecondsText.SetActive(true);
                        TimerText.text = currentTime.ToString();
                    }
                    break;

                case 1:
                    currentTimePoints += (int)(timePointsStep * directionInput.x);
                    currentTimePoints = currentTimePoints < minTimePoints ? minTimePoints : (currentTimePoints > maxTimePoints ? maxTimePoints : currentTimePoints);

                    TimerPointsText.text = currentTimePoints.ToString();

                    if (invertedCurrentSpeed > InvertedTimePointsMaxSpeed && directionInput.x != 0)
                    {
                        invertedCurrentSpeed *= SpeedIncrease;
                    }

                    break;
                case 2:
                    currentPoints += (int)(pointsStep * directionInput.x);
                    currentPoints = currentPoints < minPoints ? minPoints : (currentPoints > maxPoints ? maxPoints : currentPoints);

                    PointsText.text = currentPoints.ToString();

                    if (invertedCurrentSpeed > InvertedPointsMaxSpeed && directionInput.x != 0)
                    {
                        invertedCurrentSpeed *= SpeedIncrease;
                    }

                    break;
                case 3:
                    if(currentReqPoints == minReqPoints)
                    {
                        currentReqPoints = 0;
                    }

                    currentReqPoints += (int)(reqPointsStep * directionInput.x);
                    currentReqPoints = currentReqPoints < minReqPoints ? minReqPoints : (currentPoints > maxReqPoints ? maxReqPoints : currentReqPoints);

                    ReqPointsText.text = currentReqPoints.ToString();

                    if (invertedCurrentSpeed > InvertedReqPointsMaxSpeed && directionInput.x != 0)
                    {
                        invertedCurrentSpeed *= SpeedIncrease;
                    }

                    break;
            }

            

            TimeInPhase = 0;

            if ((currentTime < minTime || currentTimePoints <= 0) && currentPoints <= 0)
            {
                WarningField.SetActive(true);
            }
            else { WarningField.SetActive(false); }
        }
    }
    public void Update()
    {
        if (settingsStarted)
        {
            DoSettingsUpdate();
        }
    }
    public void ResetSettings()
    {
        currentSelectedField = 0;
        currentPoints = pointsStartValue;
        currentTimePoints = timePointsStartValue;
        currentTime = timeStartValue;
        currentReqPoints = reqPointsStartValue;

        TimerText.text = currentTime.ToString();
        TimerPointsText.text = timePointsStartValue.ToString();
        PointsText.text = currentPoints.ToString();
        ReqPointsText.text = currentReqPoints.ToString();

        if (FinishedSetupEvent != null)
        {
            FinishedSetupEvent.Invoke(this, EventArgs.Empty);
        }
    }
    public void StartSettingsPhase()
    {
        PointsText.text = currentPoints.ToString();
        ReqPointsText.text = currentReqPoints.ToString();
        TimerPointsText.text = currentTimePoints.ToString();
        TimerText.text = currentTime.ToString();

        OptionField.SetActive(true);
        TimeInPhase = 0;

        if (PlayerConfigManager.Instance != null)
        {
            SetPlayerInput(PlayerConfigManager.Instance.GetPlayerConfigs()[0]);
        }
        else
        {
            if (controller == null)
            {
                controller = new PlayerInputMap();
                GetComponent<PlayerInput>().onActionTriggered += Input_onActionTriggered;
            }
        }
        settingsStarted = true;

        foreach (GameObject gameObject in SelectArrows)
        {
            gameObject.SetActive(false);
        }
        currentSelectedField = 0;
        SelectArrows[currentSelectedField].SetActive(true);
        directionInput = Vector2.zero;

        if (AToJoinText != null)
        {
            AToJoinText.SetActive(false);
        }
    }
    private void SetPlayerInput(PlayerConfig playerConfig)
    {
        if(playerConfig == lastPlayer) { return; }

        controller = new PlayerInputMap();

        playerConfig.Input.onActionTriggered += Input_onActionTriggered;
        if (lastPlayer != null)
        {
            lastPlayer.Input.onActionTriggered -= Input_onActionTriggered;
        }

        lastPlayer = playerConfig;
    }

    private void Input_onActionTriggered(CallbackContext obj)
    {
        if(!settingsStarted) { return; }

        if (obj.action.name == controller.Player.Move.name)
        {
            OnDirectionInput(obj.ReadValue<Vector2>());
        }
        if (obj.action.name == controller.Player.Talk.name)
        {
            OnDirectionInput(obj.ReadValue<Vector2>());
        }
        if (obj.action.name == controller.Player.Confirm.name)
        {
            Confirm();
        }
        if (obj.action.name == controller.Player.Decline.name)
        {
            Cancel();
        }
    }
    private void OnDirectionInput(Vector2 direction)
    {
        if (!settingsStarted) { return; }
        direction.x = Mathf.Abs(direction.x) > Mathf.Abs(direction.y) ? direction.x : 0;
        direction.y = Mathf.Abs(direction.y) >= Mathf.Abs(direction.x) ? direction.y : 0;


        direction.x = direction.x > 0 ? 1 : (direction.x < 0 ? -1 : 0);
        direction.y = direction.y > 0 ? -1 : (direction.y < 0 ? 1 : 0); //kinda weird but makes sense trust

        if (direction.x != 0)
        {
            TimeInPhase = direction.x != directionInput.x? 2 : TimeInPhase; //I want instant feedback so 2 instead of 0.
            invertedCurrentSpeed = direction.x != directionInput.x? InvertedStartSpeed : invertedCurrentSpeed;
        }
        if (direction.y != 0 && direction.y != directionInput.y)
        {
            SelectArrows[currentSelectedField].SetActive(false);

            currentSelectedField += (int)direction.y;
            currentSelectedField = currentSelectedField < 0 ? optionCount : (currentSelectedField > optionCount ? 0 : currentSelectedField);

            SelectArrows[currentSelectedField].SetActive(true);
        }

        directionInput = direction;
    }
    public void Cancel()
    {
        if (PlayerConfigManager.Instance != null)
        {
            PlayerConfigManager.Instance.UnReadyAll();
        }
        if (AToJoinText != null)
        {
            AToJoinText.SetActive(true);
        }
        OptionField.SetActive(false);
        settingsStarted = false;
    }
    private void Confirm()
    {
        if (currentSelectedField == optionCount)
        {
            settingsStarted = false;

            GameSettingsManager.Instance.SetGameTime(currentTime < minTime ? -1 : currentTime); // can be -1, represents no timer.
            GameSettingsManager.Instance.SetPointsForPlacement(currentPoints);
            GameSettingsManager.Instance.SetPointsPerSecond(currentTimePoints / currentTime);
            GameSettingsManager.Instance.SetRequiredPoints(currentReqPoints);

            if(FinishedSetupEvent != null)
            {
                FinishedSetupEvent.Invoke(this, EventArgs.Empty);
            }
            //PlayerConfigManager.Instance.StartGame();
            //OptionField.SetActive(false);
            if (PlayerConfigManager.Instance != null)
            {
                settingsAnim.SetTrigger("LoadGame");
            }
            else
            {
                Cancel();
            }
            if (loadingScript != null)
            {
                loadingScript.LoadScene(Loader.GetSceneID(SceneToLoad));
            }
        }
    }
    public void LoadDefaultSettings()
    {
        GameSettingsManager.Instance.SetGameTime(currentTime < minTime ? -1 : currentTime); // can be -1, represents no timer.
        GameSettingsManager.Instance.SetPointsForPlacement(currentPoints);
        GameSettingsManager.Instance.SetPointsPerSecond(currentTimePoints / currentTime);
        GameSettingsManager.Instance.SetRequiredPoints(currentReqPoints);

        FinishedSetupEvent.Invoke(this, EventArgs.Empty);
    }
}
