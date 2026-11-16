using System.Linq;
using UnityEngine;

public class FeetScript : MonoBehaviour
{
    [SerializeField] BoxCollider FeetBox;
    bool isGrounded = true;
    [SerializeField] int alreadyExistingCols;
    [SerializeField] LayerMask cantStandLayers;
    
    public bool GetGrounded()
    {
        return isGrounded;
    }
    private void FixedUpdate()
    {
        isGrounded = false;
        var colliders = Physics.OverlapBox(FeetBox.transform.position, FeetBox.size / 2 * transform.parent.lossyScale.x, transform.rotation, ~cantStandLayers);

        if (colliders.Count() > alreadyExistingCols)
        {
            isGrounded = true;
        }
    }
    public void Respawn()
    {
        isGrounded = true;
    }
    //private void OnTriggerStay(Collider other)
    //{
    //    groundCounter = 1;

    //    GroundCheck();
    //}

}
