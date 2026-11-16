using UnityEngine;

public class DeathPlane : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PlayerManager>() != null)
        {
            if (other.GetComponent<PlayerManager>().enabled)
            {
                other.GetComponent<PlayerManager>().DecreaseHP(10000);
            }
        }
    }
}
