using UnityEngine;

public class CannonBallScript : MonoBehaviour
{

    public void Fire(Transform target, float velocity) {
        transform.position = transform.parent.position;
        transform.rotation = transform.parent.rotation;
        Vector3 direction = (target.position - transform.position).normalized;

        GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        GetComponent<Rigidbody>().AddForce(direction * velocity, ForceMode.Impulse);
    }
    void Update()
    {
        
    }
}
