using System.Collections.Generic;
using UnityEngine;

public class GameManagerAdapter : GameManager
{
    //note this is mainly just a helper for the server game manager. it exists to parse values through an override in the checkFinished Logic
    public override void Start()
    {
    }
    public override void CheckFinished()
    {
        Debug.Log("Finish");
        if (ServerGameManager.Instance.currentState == ServerGameManager.State.parkourState)
        {
            Debug.Log("HEereere");
            ServerGameManager.Instance.FinishPlayerCamSwap();
            ServerGameManager.Instance.CheckFinishedServerRpc(PlayerInteractionManager.Instance.GetInstanceID());
        }
        else {
            ServerGameManager.Instance.CheckFinishedServerRpc(-1);
        }
    }
    public override int getPlacedItemCount()
    {
        return 0;
    }
}
