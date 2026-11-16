using UnityEngine;
using UnityEngine.UIElements;

public class AttachScript : MonoBehaviour
{
    private GameObject attachedObject;
    private GameObject attachedToObject;
    private Vector3 attachedOffset;
    private Vector3 attachedRotation;
    public bool attached {get; private set;} = false;
    public void Attach(GameObject attacher, GameObject attachedTo, Vector3 offset, Vector3 rotationOffset)
    {
        if (!attached)
        {
            attachedObject = attacher;
            attachedToObject = attachedTo;
            attachedOffset = offset;
            attachedRotation = rotationOffset;

            //attachedObject.transform.Rotate(Vector3.up, rotationOffset.y);
            //attachedObject.transform.Rotate(Vector3.left, rotationOffset.x);
            //attachedObject.transform.Rotate(Vector3.forward, rotationOffset.z);

            attached = true;
        }
    }

    public GameObject GetAttachedObject()
    {
        return attachedObject;
    }
    public GameObject GetAttachedToObject()
    {
        return attachedToObject;
    }
    public void Release()
    {
        attached = false;
    }
    private void LateUpdate()
    {
        if(attachedObject == null)
        {
            attached = false ;
            return;
        }
        if (attached)
        {
            attachedObject.transform.position = attachedToObject.transform.position + attachedToObject.transform.TransformVector(attachedOffset);
            attachedObject.transform.rotation = attachedToObject.transform.rotation;

            attachedObject.transform.Rotate(attachedToObject.transform.up, attachedRotation.y);
            attachedObject.transform.Rotate(attachedToObject.transform.right, attachedRotation.x);
            attachedObject.transform.Rotate(attachedToObject.transform.forward, attachedRotation.z);
        }

    }
    private void OnDisable()
    {
        Release();
    }
}
