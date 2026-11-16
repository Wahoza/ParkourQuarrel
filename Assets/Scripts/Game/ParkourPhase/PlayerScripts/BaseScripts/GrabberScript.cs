using System.Collections.Generic;
using UnityEngine;

public class GrabberScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] Rigidbody playerRb;
    [SerializeField] SphereCollider grabberTrigger;
    [SerializeField] GameObject pocket;
    [SerializeField] public AttachScript attacher;
    [SerializeField] float grabTime;
    private float timeUntilGrabEnd;
    IGrabbableScript grabbedItem = null;
    bool grabbing = false;
    float grabbedMass = 0;

    public bool isOnline = false;
    void Start()
    {

    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (grabbing) {
            if (timeUntilGrabEnd < Time.time)
            {
                grabberTrigger.enabled = false;
                grabbing = false;
            }
        }

        
        if (grabbedItem != null) 
        {
            grabbedItem.GrabUpdate();
        }
    }
    public void GrabAttempt() {
        timeUntilGrabEnd = Time.time + grabTime;
        grabberTrigger.enabled = true;
        grabbing = true;
    }
    public virtual void Release() {

        attacher.Release();

        grabbedItem?.OnRelease(gameObject);
        playerRb.mass -= grabbedMass;
        grabbedMass = 0;

        grabbedItem = null;
    }
    public virtual void Throw()
    {
        grabbedItem?.OnThrow(gameObject);

        attacher.Release();

        playerRb.mass -= grabbedMass;
        grabbedMass = 0;

        grabbedItem = null;

        Debug.Log("Throw");
    }
    private void OnTriggerStay(Collider other)
    {
        if (!isOnline)
        {
            doGrabFunc(other);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!isOnline)
        {
            doGrabFunc(other);
        }
    }
    public virtual void doGrabFunc(Collider other)
    {
        if (grabbedItem != null) { return; }

        if (other.GetComponent<IGrabbableScript>() != null)
        {
            grabbedItem = other.GetComponent<IGrabbableScript>();
            playerRb.mass += grabbedItem.GetWeight();
            grabbedMass = grabbedItem.GetWeight();

            other.transform.position = pocket.transform.position;

            grabbedItem.OnGrab(gameObject);

            if (grabbedItem != null)
            {
                //other.transform.parent = pocket.transform;
                attacher.Attach(other.gameObject, pocket, other.GetComponent<IGrabbableScript>().GrabOffset, other.GetComponent<IGrabbableScript>().RotationOffset);

                grabberTrigger.enabled = false;

                grabbedItem.OnGrabFinished(gameObject);
            }
        }
    }
    public bool IsHolding()
    {
        return attacher.attached;    
    }

    public void ManualChangeCollider(bool colliderState)
    {
        grabberTrigger.enabled = false;
    }

}
