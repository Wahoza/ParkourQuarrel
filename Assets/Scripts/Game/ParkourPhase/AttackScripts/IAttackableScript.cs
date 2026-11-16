using UnityEngine;

public class IAttackableScript : MonoBehaviour
{
    public virtual void OnAttack(GameObject attacker, int damage, float hitStrength, Vector3 knockBackDir)
    {

    }
}
