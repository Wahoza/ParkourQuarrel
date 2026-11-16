using UnityEngine;

public class GrabbableCube : IGrabbableScript
{
    [SerializeField] public float throwStrength;
    [SerializeField] float weight;

    public override void OnGrab(GameObject grabber)
    {

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
        rb.AddForce(force, ForceMode.Impulse);

    }
    public override float GetWeight()
    {
        return weight;
    }
}
