using UnityEngine;

public class LatcherScript : MonoBehaviour
{
    [SerializeField] GameObject Latcher;
    [SerializeField] Transform MyToLatch;
    [SerializeField] private AttachScript attachScript;

    Rigidbody parentRB;
    Rigidbody myRB;

    Transform origin;
    bool isLatched = false;

    //bool latchedWithRB;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] LayerMask ignoredLayers;
    private void Start()
    {
        isLatched = false;
        TryLatch();
    }
    void TryLatch()
    {
        origin = transform.parent;
        //latchedWithRB = false;
        if (Physics.Raycast(Latcher.transform.position, -Latcher.transform.up, out RaycastHit hit, 2f, ~ignoredLayers))
        {
            isLatched = true;
            Debug.Log(hit.transform.gameObject.name);
            attachScript.Attach(MyToLatch.gameObject, hit.transform.gameObject, MyToLatch.transform.position - hit.transform.position + Latcher.transform.up * -0.1f, Vector3.zero);

            //MyToLatch.parent = hit.collider.transform;

            Debug.Log(transform.parent.gameObject.name);
            MyToLatch.transform.position = hit.point + Latcher.transform.up * 0.1f + (MyToLatch.transform.position - Latcher.transform.position);
            parentRB = transform.parent.GetComponent<Rigidbody>();
            myRB = transform.GetComponent<Rigidbody>();
            if (parentRB != null && myRB != null)
            {
                //latchedWithRB = true;
            }
        }
    }

    private void FixedUpdate()
    {
        //if (latchedWithRB)
        //{
        //    parentRB.AddForceAtPosition(myRB.linearVelocity, transform.position, ForceMode.Force);
        //}
    }

    //private void OnDisable()
    //{
    //    //Debug.Log(isLatched);
    //    //Debug.Log("DisableAttempt Above");
    //    if (isLatched)
    //    {
    //        if (origin != null)
    //        {
    //            MyToLatch.transform.parent = origin;
    //            MyToLatch.transform.position = origin.position;
    //        }

    //        isLatched = false;
    //    }
    //}
}
