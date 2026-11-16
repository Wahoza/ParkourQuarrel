using Unity.Netcode;
using UnityEngine;

public class HandID : NetworkBehaviour
{
    public int ID {  get; private set; }
    [ServerRpc(RequireOwnership = false)]
    public void SetIDServerRpc(int id) 
    {
        SetIDClientRpc(id);
    }
    [ClientRpc]
    private void SetIDClientRpc(int id)
    {
        ID = id;

        PlayerInteractionManager.Instance.JoinDictionaryHand(gameObject);
        ServerGameManager.Instance.RegisterHand(GetComponent<HandControllerOnline>());
    }
}
