using Unity.VisualScripting;
using UnityEngine;

public class GrabbableBread : IGrabbableScript
{
    [SerializeField] int healthGain;
    [SerializeField] float sizeGain;
    [SerializeField] float jumpGain;
    [SerializeField] GameObject model;
    [SerializeField] Collider myHitbox;
    private bool isEaten;


    private void ResetObject()
    {
        isEaten = false;
        model.SetActive(true);
        myHitbox.enabled = true;
    }

    public override void OnGrab(GameObject grabber)
    {
        var player = grabber.transform.parent;
        if (player != null && !isEaten) {
            player.localScale *= sizeGain;
            
            player.gameObject.GetComponent<PlayerManager>().IncreaseHP(healthGain);
            player.gameObject.GetComponent<Rigidbody>().mass *= sizeGain;
            player.gameObject.GetComponent<PlayerController>().IncreaseJumpStrenght(jumpGain);


            myHitbox.enabled = false;
            model.SetActive(false);
            isEaten = true;
        }
    }
}
