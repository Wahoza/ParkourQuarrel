using UnityEngine;

public class IGrabbableScript : MonoBehaviour
{
    [SerializeField] public Vector3 GrabOffset;
    [SerializeField] public Vector3 RotationOffset;
    public virtual void OnGrab(GameObject grabber)
    {
    }
    public virtual void OnGrabFinished(GameObject grabber) 
    { 
    }
    public virtual void OnRelease(GameObject grabber)
    {
    }
    public virtual void OnThrow(GameObject grabber)
    {
    }
    public virtual void GrabUpdate()
    {
    }
    public virtual float GetWeight() {
        return 0f;
    }
}
