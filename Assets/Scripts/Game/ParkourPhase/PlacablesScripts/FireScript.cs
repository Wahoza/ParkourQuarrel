using UnityEngine;

public class FireScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] float moveSpeed;
    [SerializeField, Range(0, 1)] float moveImpact;
    [SerializeField] float maxTTL;
    
    private float instanceTTL;
    [SerializeField] Vector3 moveDir;
    [SerializeField] Vector3 startGrowthSize;
    [SerializeField] Vector3 endGrowthSize;
    [SerializeField] float growthSpeed;
    [SerializeField] Rigidbody myRB;
    [SerializeField] Transform myFire;
    private Vector3 mixedVelocity;
    void Start()
    {
        ResetFire();
    }
    public void ResetFire()
    {
        instanceTTL = Time.time + maxTTL;
        transform.localScale = startGrowthSize;
        myRB.linearVelocity = Vector3.zero;
        myRB.angularVelocity = Vector3.zero;
        moveDir = Vector3.zero; 
    }
    public void SetMoveDir(Vector3 direction)
    {
        moveDir = direction;
    }

    // Update is called once per frame
    void Update()
    {
        mixedVelocity = new Vector3 (moveDir.x * moveSpeed, myRB.linearVelocity.y, moveDir.z * moveSpeed);
        myRB.linearVelocity = Vector3.Lerp(myRB.linearVelocity, mixedVelocity , moveImpact);
        myFire.localScale = Vector3.Lerp(transform.localScale, endGrowthSize, growthSpeed * Time.deltaTime);
        if (instanceTTL < Time.time)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 4)
        {
            gameObject.SetActive(false);
        }
    }
}
