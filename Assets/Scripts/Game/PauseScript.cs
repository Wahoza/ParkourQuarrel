using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

public class PauseScript : MonoBehaviour
{
    [SerializeField] GameObject myPauseMenu;
    [SerializeField] TextMeshProUGUI pauserNameText;
    [SerializeField] List<TextMeshProUGUI> OptionsTexts;
    [SerializeField] List<string> OptionsStrings;
    [SerializeField] Image QuitCircle;

    private PlayerConfig playerConfigPauser;
    private PlayerConfig playerOneConfig;
    private PlayerInputMap controls;

    [SerializeField] private float holdForQuitDuration;

    private int currentSelection = 0;
    private int lastDir = 0;
    private bool isButtonDown = false;
    private bool isPaused = false;

    public void Pause(PlayerConfig pc) 
    {
        return;
        isPaused = true;

        for (int i = 0; i < OptionsTexts.Count; i++)
        {
            OptionsTexts[i].text = OptionsStrings[i];
        }

        isButtonDown = false;
        currentSelection = 0;
        OptionsTexts[currentSelection].text = $"<u>{OptionsStrings[currentSelection]}</u>";

        //SwapSelection(0);

        playerConfigPauser = pc;
        playerConfigPauser.Input.onActionTriggered += Input_onActionTriggered;
        controls = new PlayerInputMap();

        Time.timeScale = 0f;
        QuitCircle.fillAmount = 0;

        pauserNameText.text = $"P{(pc.PlayerIndex + 1)}";
        myPauseMenu.SetActive(true);
    }
    private void Resume()
    {
        isPaused = false;

        playerConfigPauser.Input.onActionTriggered -= Input_onActionTriggered;

        Time.timeScale = 1f;
        myPauseMenu.SetActive(false);
    }
    private void Quit()
    {
        isPaused = false;

        playerConfigPauser.Input.onActionTriggered -= Input_onActionTriggered;

        Time.timeScale = 1f;
        myPauseMenu.SetActive(false);

        PlayerConfigManager.Instance.ClearPlayers();
        Loader.LoadLocal(Loader.Scenes.MainMenu);
    }
    public void Input_onActionTriggered(CallbackContext obj)
    {
        if(!isPaused) { return; }

        if(obj.action.name == controls.Player.Move.name || obj.action.name == controls.Player.Talk.name)
        {
            OnMove(obj);
        }
        if (obj.action.name == controls.Player.Confirm.name)
        {
            OnAPress(obj);
        }
        if(obj.action.name == controls.Player.Pause.name)
        {
            Resume();
        }
    }
    private void OnMove(CallbackContext context)
    {
        Vector3 currentDirInput = context.ReadValue<Vector2>();

        currentDirInput = new Vector2((currentDirInput.x < 0 ? 1 : currentDirInput.x > 0 ? -1 : 0) , 0);

        SwapSelection((int)currentDirInput.x);
    }
    private void OnAPress(CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            isButtonDown = true;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            isButtonDown = false;
            QuitCircle.fillAmount = 0;
        }
    }
    void SwapSelection(int dir)
    {
        if(isButtonDown) { return; }

        if(dir == lastDir) { return; }

        lastDir = dir;

        OptionsTexts[currentSelection].text = OptionsStrings[currentSelection];
        currentSelection += dir;

        int maxSize = (OptionsTexts.Count - 1);
        currentSelection = currentSelection > maxSize ? 0 : currentSelection < 0 ? maxSize : currentSelection;

        OptionsTexts[currentSelection].text = $"<u>{OptionsStrings[currentSelection]}</u>";
    }

    private void Update()
    {
        if (!Application.isFocused) { 
            Pause(playerOneConfig);
        }


        if (!isButtonDown) { return ; }
        if (!isPaused) { return ; }
        switch (currentSelection) {
            case 0:
                Resume();
                return;
            case 1:
                QuitCircle.fillAmount += 1 / holdForQuitDuration * Time.unscaledDeltaTime;

                if (QuitCircle.fillAmount >= 1) {
                    Quit();
                }
                return;
        }
    }
    public void SetPlayerOneConfig(PlayerConfig pc)
    {
        playerOneConfig = pc;
    }
}
