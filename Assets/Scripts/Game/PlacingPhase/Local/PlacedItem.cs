using UnityEngine;

public class PlacedItem : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //The plan is to have an Object that gets instantiated and then later deleted again to reset everything. This is due to me planning on having Objects that are ok to call delete on themselves without shit going to fuck.
    bool isKinematic = false;

    GameObject Item;
    public void Initialize(GameObject PlacedItem)
    {
        gameObject.tag = "Spawner";
        transform.position = PlacedItem.transform.position;
        transform.rotation = PlacedItem.transform.rotation;
        transform.localScale = PlacedItem.transform.localScale;
        PlacedItem.transform.parent = transform;

        if(PlacedItem.GetComponent<Rigidbody>() != null)
        {
            //this is to esentialy pause the rigidbody, makes for easier checking as long as i implement the rest properly (please god)

            isKinematic = PlacedItem.GetComponent<Rigidbody>().isKinematic;
            PlacedItem.GetComponent<Rigidbody>().isKinematic = true;

        }

        GameManager.Instance.AddPlacedItem(gameObject);
    }

    public void StartParkourState()
    {
        //GETTING AN ERROR HERE IS ILLEGAL!!!!! THIS DOESNT NEED FIXING EVER
        transform.GetChild(0).gameObject.SetActive(false);

        Item = Instantiate(transform.GetChild(0).gameObject, transform.position, transform.rotation);

        Item.SetActive(true);
        Item.GetComponent<PhaseBehavior>().StartParkourPhase();
    }
    public void StartPlaceState()
    {
        if (Item != null)
        {
            Destroy(Item);
        }

        if(transform.childCount > 0)
        {
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(0).GetComponent<PhaseBehavior>().StartPlacingPhase();
        }

        

        //transform.GetChild(0).gameObject.GetComponent<Rigidbody>().isKinematic = true;
    }
    public void EraseItem()
    {
        GameManager.Instance.RemovePlacedItem(gameObject);
    }
    public void DestroyMyItem()
    {
        if(Item != null)
        {
            Destroy(Item);
        }
    }
}
