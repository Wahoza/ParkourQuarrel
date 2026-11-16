using Unity.Netcode;
using UnityEngine;

public class PlacedItemOnline : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //The plan is to have an Object that gets instantiated and then later deleted again to reset everything. This is due to me planning on having Objects that are ok to call delete on themselves without shit going to fuck.
    bool isKinematic = false;
    bool isNetworkObject = true;
    GameObject Item;

    int ObjectID = 0;


    //This method only gets Called by Server Upon Creation
    [ClientRpc]
    public void SetIDClientRpc(int id)
    {
        ObjectID = id;
        gameObject.tag = "Spawner";

        ServerGameManager.Instance.RegisterSpawnedObject(gameObject, id);
    }

    [ServerRpc(RequireOwnership = false)]
    public void InitializeServerRpc(int gameObjectID, Vector3 position, Quaternion rotation)//GameObject PlacedItem)
    {
        transform.position = position;
        transform.rotation = rotation;
        transform.localScale = Vector3.one;

        if (ServerGameManager.Instance.GetPlacableObjectOnlineFromID(gameObjectID).GetComponent<NetworkObject>() != null)
        {
            Item = Instantiate(ServerGameManager.Instance.GetPlacableObjectOnlineFromID(gameObjectID), position, rotation);


            Item.transform.parent = transform;
            Item.GetComponent<NetworkObject>().Spawn();

            isNetworkObject = true;
        }
        else
        {
            InitializeClientRpc(gameObjectID, position, rotation);
        }

        //transform.position = PlacedItem.transform.position;
        //transform.rotation = PlacedItem.transform.rotation;
        //transform.localScale = PlacedItem.transform.localScale;
        //PlacedItem.transform.parent = transform;

        transform.GetChild(0).gameObject.GetComponent<PhaseBehavior>().StartInitialPlacingPhase();
    }

    [ClientRpc]
    private void InitializeClientRpc(int gameObjectID, Vector3 position, Quaternion rotation)
    {
        isNetworkObject = false;


        Item = Instantiate(ServerGameManager.Instance.GetPlacableObjectOnlineFromID(gameObjectID), position, rotation);
        Item.transform.parent = transform;
    }



    [ServerRpc]
    public void StartParkourStateServerRpc()
    {
        if (isNetworkObject)
        {
            transform.GetChild(0).gameObject.GetComponent<PhaseBehavior>().StartParkourPhase();
        }
         
        {
            StartParkourStateClientRpc();
        }
    }
    [ClientRpc]
    public void StartParkourStateClientRpc()
    {
        if (true) { 

            //GETTING AN ERROR HERE IS ILLEGAL!!!!! THIS DOESNT NEED FIXING EVER
            transform.GetChild(0).gameObject.GetComponent<PhaseBehavior>().StartParkourPhase();
        }
    }

    [ServerRpc]
    public void StartPlacingStateServerRpc()
    {
        if (isNetworkObject)
        {
            transform.GetChild(0).gameObject.GetComponent<PhaseBehavior>().StartPlacingPhase();
        }
        
        {
            StartPlacingStateClientRpc();
        }
    }
    [ClientRpc]
    public void StartPlacingStateClientRpc()
    {
        if (true)
        {
            transform.GetChild(0).gameObject.GetComponent<PhaseBehavior>().StartPlacingPhase();
        }
    }


    public void EraseItem()
    {
        EraseItemServerRpc();
    }

    [ServerRpc(RequireOwnership =false)]
    public void EraseItemServerRpc()
    {
        if (isNetworkObject)
        {
            Destroy(transform.GetChild(0));
        }

        EraseItemClientRpc();
        ServerGameManager.Instance.RequestDestroyObject(ObjectID);
    }
    [ClientRpc]
    public void EraseItemClientRpc()
    {
        if (!isNetworkObject)
        {
            Destroy(transform.GetChild(0));
        }
        ServerGameManager.Instance.RequestDestroyObject(ObjectID);
    }
    public void DestroyMyItem()
    {
        if(Item != null)
        {
            Destroy(Item);
        }
    }


    public void ResetGameObject()
    {
        RecursiveResetPosition(transform);
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(0).gameObject.GetComponent<PhaseBehavior>().StartPlacingPhase();
    }

    private void RecursiveResetPosition(Transform origin)
    {
        for (int i = 0; i < origin.childCount; i++)
        {
            RecursiveResetPosition(origin.GetChild(i));
            origin.GetChild(i).localPosition = Vector3.zero;
            origin.GetChild(i).localPosition = Vector3.zero;
        }
    }
}
