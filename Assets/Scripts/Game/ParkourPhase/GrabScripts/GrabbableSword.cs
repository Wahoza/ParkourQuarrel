using UnityEngine;

public class GrabbableSword : IGrabbableScript
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] GameObject bladeCollider;

    [SerializeField] float throwStrength;
    [SerializeField] float rotationSpeed;

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void OnGrab(GameObject grabber)
    {
        transform.rotation = grabber.transform.rotation * Quaternion.Euler(RotationOffset);//Quaternion.identity;
        transform.localPosition += GrabOffset;

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    public override void OnRelease(GameObject grabber)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
    }

    public override void OnThrow(GameObject grabber)
    {
        Vector3 force = grabber.transform.forward * throwStrength;
        Rigidbody rb = GetComponent<Rigidbody>();

        rb.isKinematic = false;
        rb.AddTorque(new Vector3(0, rotationSpeed, 0));
        rb.AddForce(force, ForceMode.Impulse);
    }
}
