using UnityEngine;

public class GrabbableRigidbody : GrabbableCube
{
    [SerializeField] private Vector3 rotationOffset;
    [SerializeField] private Rigidbody myRB;
    [SerializeField] private Rigidbody ownerRB;
    private int maxSearches = 5;

    public override void OnRelease(GameObject grabber)
    {

        base.OnRelease(grabber);
    }
    public override void OnThrow(GameObject grabber)
    {

        base.OnThrow(grabber);
    }
}
