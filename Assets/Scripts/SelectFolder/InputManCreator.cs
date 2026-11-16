using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class InputManCreator : MonoBehaviour
{
    private enum eventType
    {
        sendMessages = 0,
        brodcastMessages = 1,
        invokeUnityEvents = 2,
        invokeCsharpEvents = 3
    }
    [SerializeField] eventType notificationBehavior;
    private enum playerJoinType
    {
        onButtonPress = 0,
        onActionTriggered = 1,
        manualJoin = 2
    }
    [SerializeField] playerJoinType playerJoinBehavior;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] bool joiningEnabledByDefault;
    [SerializeField] int maxPlayerCount;

    private void Awake()
    {
        if(GetComponent<PlayerInputManager>() == null)
        {
            CreatePlayerInputManager();
        }

    }
    private void CreatePlayerInputManager()
    {
        gameObject.AddComponent<PlayerInputManager>();

        var manager = gameObject.GetComponent<PlayerInputManager>();
        manager.playerPrefab = playerPrefab;
        switch (notificationBehavior)
        {
            case eventType.sendMessages:
                manager.notificationBehavior = PlayerNotifications.SendMessages;
                return;
            case eventType.brodcastMessages:
                manager.notificationBehavior = PlayerNotifications.BroadcastMessages;
                return;
            case eventType.invokeUnityEvents:
                manager.notificationBehavior = PlayerNotifications.InvokeUnityEvents;
                return;
            case eventType.invokeCsharpEvents:
                manager.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;
                return;
        }


        switch (playerJoinBehavior)
        {
            case playerJoinType.onButtonPress:
                manager.joinBehavior = PlayerJoinBehavior.JoinPlayersWhenButtonIsPressed;
                return;
            case playerJoinType.onActionTriggered:
                manager.joinBehavior = PlayerJoinBehavior.JoinPlayersWhenJoinActionIsTriggered;
                return;
            case playerJoinType.manualJoin:
                manager.joinBehavior = PlayerJoinBehavior.JoinPlayersManually;
                return;
        }

        if (joiningEnabledByDefault)
        {
            manager.EnableJoining();
        }
        else
        {
            manager.DisableJoining();
        }

        //Replace decleration in InputManager thingy
        ////    public int maxPlayerCount
        //{
        //    get
        //    {
        //        return m_MaxPlayerCount;
        //    }
        //    set
        //    {
        //        m_MaxPlayerCount = value;
        //    }
        //}
        manager.maxPlayerCount = maxPlayerCount;
    }
}
