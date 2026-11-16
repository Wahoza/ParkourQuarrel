using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class PQLobby : MonoBehaviour
{
    public static PQLobby Instance {get; private set;}
    private Lobby joinedLobby;
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitUnityAuthentication();
    }

    private async void InitUnityAuthentication()
    {
        if(UnityServices.State == ServicesInitializationState.Initialized) { return; }

        InitializationOptions options = new InitializationOptions();
        options.SetProfile(Random.Range(0, 10000).ToString());
        await UnityServices.InitializeAsync();

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void CreateLobby(string lobbyName, bool isPrivate)
    {
        try
        {
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, OnlineGameFlowManager.maxPlayerCount, new CreateLobbyOptions
            {
                IsPrivate = isPrivate,
            });

            OnlineGameFlowManager.Instance.StartHost();
            Loader.LoadNetwork(Loader.Scenes.CharacterSelectSceneOnline);
        }
        catch (LobbyServiceException ex)
        {
            Debug.Log(ex);
        }
    }

    public async void QuickJoin()
    {
        try
        {
            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
            OnlineGameFlowManager.Instance.StartClient();

        }
        catch (LobbyServiceException ex)
        {
            Debug.Log(ex);
        }
    }
}
