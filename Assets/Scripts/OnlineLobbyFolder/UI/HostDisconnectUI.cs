using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class HostDisconnectUI : MonoBehaviour
{
    [SerializeField] GameObject QuitUI;
    [SerializeField] EventSystem eventSystem;
    [SerializeField] GameObject QuitButton;
    private void Start()
    {
        QuitUI.SetActive(false);

        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        Debug.Log("Kick");

        if (clientId == NetworkManager.ServerClientId) 
        {
            Debug.Log("Server DC");
            Show();
            eventSystem.SetSelectedGameObject(QuitButton);

            Time.timeScale = 0;
        }
        else if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            Show();
            eventSystem.SetSelectedGameObject(QuitButton);
        }
    }

    public void QutiToHome()
    {
        Loader.LoadLocal(Loader.Scenes.MainMenu);
    }
    private void Show()
    {
        QuitUI.SetActive(true);
    }
    private void Hide()
    {
        QuitUI.SetActive(false);
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_OnClientDisconnectCallback;
        }
    }
    }
