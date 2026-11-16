using UnityEngine;

public class BoulderScript : MonoBehaviour
{
    [SerializeField] float Velocity;
    [SerializeField] Rigidbody rb;
    Vector3 direction = Vector3.zero;
    private void Awake()
    {
        direction = transform.forward;

    }
    private void Update()
    {
        if (rb.isKinematic) { return; }
        rb.AddForce(direction * Velocity * Time.deltaTime, ForceMode.Acceleration);

    }
}
