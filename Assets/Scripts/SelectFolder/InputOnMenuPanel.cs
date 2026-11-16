using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.Windows;
using static UnityEngine.InputSystem.InputAction;

public class InputOnMenuPanel : MonoBehaviour
{

    private InputSystemUIInputModule uiInputModule;
    [SerializeField] private PlayerSetupMenuController controller;
    [SerializeField] private ReadyScript readyController;
    Vector2Int lastNavigationInputChange;
    private void Start()
    {
        // Access the Event System
        EventSystem eventSystem = GetComponent<EventSystem>();

        // Get the InputSystemUIInputModule
        uiInputModule = eventSystem.GetComponent<InputSystemUIInputModule>();
        if (uiInputModule == null)
        {
            Debug.LogError("InputSystemUIInputModule not found on EventSystem.");
        }
    }

    private void Update()
    {
        if (uiInputModule != null)
        {
            var navigationInput = uiInputModule.move.action?.ReadValue<Vector2>() ?? Vector2.zero;


            if (navigationInput.x != 0 || navigationInput.y != 0)
            {
                int position = Math.Abs(navigationInput.x) > Math.Abs(navigationInput.y) ? 0 : 1;
                Vector2Int newNavigation = Vector2Int.zero;
                newNavigation[position] = navigationInput[position] > 0 ? 1 : -1;

                if (newNavigation != lastNavigationInputChange)
                {
                    lastNavigationInputChange = newNavigation;
                    controller.ChangeSelection(newNavigation);
                }
            }
            else
            {
                lastNavigationInputChange = Vector2Int.zero;
            }



            if (uiInputModule.submit.action?.triggered == true)
            {
                controller.SelectButtonDown();
            }
            if (uiInputModule.cancel.action?.triggered == true)
            {
                readyController.CancelReady();
            }
        }
    }
}

