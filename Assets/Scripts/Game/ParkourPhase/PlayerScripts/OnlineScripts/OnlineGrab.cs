using UnityEngine;

public class OnlineGrab : GrabberScript
{
    [SerializeField] OnlinePlayer player;
    private Collider attempt;

    private void Awake()
    {
        base.isOnline = true;
    }
    public override void Release()
    {
        if (!attacher.attached) { return; }
        if (attacher.GetAttachedObject().GetComponent<OnlinePlayer>() != null)
        {
            base.Release();

            PlayerInteractionManager.Instance.RequestPlayerRelease(player.ID, attacher.GetAttachedObject().GetComponent<OnlinePlayer>().ID);
        }
    }
    public override void doGrabFunc(Collider other)
    {
        if (other.gameObject.GetComponent<OnlinePlayer>() != null)
        {
            attempt = other;
            PlayerInteractionManager.Instance.RequestPlayerGrab(player.ID, other.gameObject.GetComponent<OnlinePlayer>().ID);
        }
        else
        {
            base.doGrabFunc(other);
        }
    }
    public void RecieveGrabConfirm()
    {
        base.doGrabFunc(attempt);
    }
    public void ForceGrab(GameObject GrabbedObject)
    {
        base.doGrabFunc(GrabbedObject.GetComponent<Collider>());
    }
    public override void Throw()
    {
        if (!attacher.attached) { return; }

        if (attacher.GetAttachedObject().GetComponent<OnlinePlayer>() != null)
        {
            base.Throw();

            PlayerInteractionManager.Instance.RequestPlayerThrow(player.ID, attacher.GetAttachedObject().GetComponent<OnlinePlayer>().ID);
        }
        else
        {
            base.Throw();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        doGrabFunc(other);
    }
    private void OnTriggerEnter(Collider other)
    {
        doGrabFunc(other);
    }
    private void OnDestroy()
    {
        base.Release();
    }
}
