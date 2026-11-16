using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    float halfMaxSearchHeight = 100;
    [SerializeField] Transform SearchStart;
    [SerializeField] Transform Holder;
    [SerializeField] SphereCollider HandCollider;
    [SerializeField] LayerMask IgnoredLayers;
    public void SpawnGameObject(GameObject go)
    {
        Vector3 searchArea = go.GetComponent<BoxCollider>().size / 2;

        float searchYSkip = searchArea.y;
        searchArea.y = halfMaxSearchHeight;

        var colliders = Physics.OverlapBox(SearchStart.position , searchArea, transform.rotation, ~IgnoredLayers);

        float highestY = -100;
        foreach (Collider collider in colliders) {
            if(collider.bounds.max.y > highestY && !collider.transform.IsChildOf(Holder) && collider != HandCollider)
            {
                highestY = collider.bounds.max.y;
            }
        }


        Vector3 finalPosition = new Vector3(transform.position.x,highestY + searchYSkip + 0.2f,transform.position.z);

        var createdObject = Instantiate(go, finalPosition, transform.rotation);
        createdObject.GetComponent<PhaseBehavior>().EndInitialPlacingPhase();

        GameObject newGameObject = new GameObject("PlacedItem");
        newGameObject.AddComponent<PlacedItem>();
        newGameObject.GetComponent<PlacedItem>().Initialize(createdObject);
        newGameObject.GetComponent<PlacedItem>().StartPlaceState();
        if (Holder.GetChild(0) != null) { Destroy(Holder.GetChild(0)?.gameObject); }
    }
    public void SetHeldObject(GameObject go)
    {
        float yoffset = go.GetComponent<BoxCollider>().size.y / 2;
        go.transform.position = new Vector3(Holder.position.x , Holder.position.y - yoffset, Holder.position.z);
        go.transform.rotation = Holder.rotation;
        go.transform.parent = Holder;
    }
}
