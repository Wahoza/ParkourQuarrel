using UnityEngine;

public class GrabbablePlayer : IGrabbableScript
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private PlayerController playerScript;
    [SerializeField] private GrabberScript grabberScript;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private GameObject DecalProjector;
    [SerializeField] int JumpsToEscape;
    [SerializeField] float throwStrength;
    [SerializeField] float stunDuration;
    GameObject grabberObject;
    int JumpCounter;

    public override void OnGrab(GameObject grabber)
    {
        grabberObject = grabber;
        playerScript.CanControl(false);

        rb.isKinematic = true;
        rb.interpolation = RigidbodyInterpolation.None;
        transform.position += GrabOffset * gameObject.transform.localScale.y;
        transform.rotation = Quaternion.identity;
        JumpCounter = 0;

        DecalProjector.SetActive(false);
    }
    public override void OnRelease(GameObject grabber)
    {
        playerScript.CanControl(true);

        rb.isKinematic = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        DecalProjector.SetActive(true);

    }
    public override void OnThrow(GameObject grabber)
    {
        Vector3 force = grabber.transform.forward * throwStrength;

        playerScript.CanControl(true);

        rb.isKinematic = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        rb.AddForce(force, ForceMode.Impulse);

        playerScript.StunPlayer(stunDuration, force);
        DecalProjector.SetActive(true);
    }
    public override void GrabUpdate()
    {
        if(JumpCounter >= JumpsToEscape && grabberObject != null)
        {
            ForceEscape();
            
            rb.isKinematic = false;
            playerScript.ForceJump();
            JumpCounter = 0;
        }
    }
    public void ForceEscape()
    {
        if (grabberObject != null)
        {
            grabberObject.GetComponent<GrabberScript>().Release();
        }
    }
    public override float GetWeight()
    {
        return GetComponent<Rigidbody>().mass;
    }
    public void JumpInput()
    {
        JumpCounter++;
    }
}
