using JetBrains.Annotations;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

public class WinScreenOptions : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] List<GameObject> Options;
    [SerializeField] float requiredHoldDuration;
    private List<TextMeshProUGUI> OptionsTextItemsList;
    [SerializeField] List<string> OptionsTexts;
    [SerializeField] float ignoreInputTime;
    private float ignoreInputTimer;
    private List<Image> OptionsImages;
    private int ControllingPlayer;
    private int currentSelection;

    private PlayerInputMap controls;
    private PlayerConfig controllingPlayer;
    private Vector2 currentDirInput; 
    private Vector2 lastDirInput;

    private float currentHoldTimeElapsed = 0;
    private enum SelectionState
    {
        selecting,
        completing,
        finished
    }
    SelectionState currentState = SelectionState.selecting;
    void Awake()
    {
        controls = new PlayerInputMap();
        OptionsImages = new List<Image>();
        OptionsTextItemsList = new List<TextMeshProUGUI>();

        ignoreInputTimer = Time.time + ignoreInputTime;

        foreach (GameObject o in Options)
        {
            if(o.GetComponent<Image>() != null)
            {
                OptionsImages.Add(o.GetComponent<Image>());
            }
            if(o.transform.childCount > 0)
            {
                if (o.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>() != null)
                {
                    OptionsTextItemsList.Add(o.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>());
                }
            }
        }

        for (int i = 0; i < OptionsTexts.Count; i++) {
            OptionsTextItemsList[i].text = "<b>" + (i==0?"<u>":"") + OptionsTexts[i] + (i == 0 ? "</u>" : "") + "</b>";
        }
    }
    public void Initialize(PlayerConfig pc)
    {
        controllingPlayer = pc;
        controllingPlayer.Input.onActionTriggered += Input_onActionTriggered;
    }
    private void Input_onActionTriggered(CallbackContext obj) {
        //note: i hate doing this but i dont wanna create a new input map so here we are
        if(Time.time < ignoreInputTime || currentState == SelectionState.finished) { return; }

        if (obj.action.name == controls.Player.Move.name || obj.action.name == controls.Player.Talk.name)
        {
            OnDirectionPress(obj);
        }
        if (obj.action.name == controls.Player.Confirm.name)
        {
            OnConfirmInteraction(obj);
            //OnJump(obj);
        }
    }
    private void OnConfirmInteraction(CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            currentState = SelectionState.completing;
            currentHoldTimeElapsed = 0;
        }
        if (context.phase == InputActionPhase.Canceled)
        {
            currentState = SelectionState.selecting;

            foreach (Image image in OptionsImages)
            {
                if (image != null)
                {
                    image.fillAmount = 0;
                }
            }
        }
    }
    private void OnDirectionPress(CallbackContext context) {
        if (context.ReadValue<Vector2>() == null) { return; }

        currentDirInput = context.ReadValue<Vector2>();
        currentDirInput = new Vector2(0, currentDirInput.y < 0 ? 1 : currentDirInput.y > 0 ? -1 : 0);


        if (currentDirInput != lastDirInput && currentState == SelectionState.selecting)
        {
            OptionsTextItemsList[currentSelection].text = "<b>" + OptionsTexts[currentSelection] + "</b>";



            currentSelection += (int)currentDirInput.y;

            if (currentSelection >= Options.Count)
            {
                currentSelection = 0;
            }
            else if (currentSelection < 0)
            {
                currentSelection = Options.Count - 1;
            }


            OptionsTextItemsList[currentSelection].text = "<b><u>" + OptionsTexts[currentSelection] + "</u></b>";

        }
        lastDirInput = currentDirInput;

    }
    // Update is called once per frame
    void Update()
    {
        if (currentState == SelectionState.completing) {
            currentHoldTimeElapsed += Time.deltaTime / requiredHoldDuration;
            OptionsImages[currentSelection].fillAmount = currentHoldTimeElapsed;

            if(currentHoldTimeElapsed >= 1)
            {
                FinishOptions();
            }
        }
    }
    void FinishOptions()
    {
        Debug.Log("Finished");
        currentState = SelectionState.finished;
        
        switch (currentSelection)
        {
            case 0:
                PlayerConfigManager.Instance.ResetPlayerConfigPoints();
                SceneManager.LoadScene(2);
                return; 
            case 1:
                PlayerConfigManager.Instance.ClearPlayers();
                SceneManager.LoadScene(1);
                return;
            case 2:
                PlayerConfigManager.Instance.ClearPlayers();
                SceneManager.LoadScene(0);
                return;
        }
    }
}
