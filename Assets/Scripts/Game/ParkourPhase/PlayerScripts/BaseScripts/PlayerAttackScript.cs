using Unity.VisualScripting;
using UnityEngine;

public class PlayerAttackScript : MonoBehaviour
{
    [SerializeField] SphereCollider attackHitBox;
    [SerializeField] float attackTime;
    private float timeUntilAttackEnd;
    bool attacking = false;
    void LateUpdate()
    {
        if (!attacking) { return; }
        if (timeUntilAttackEnd < Time.time)
        {
            StopAttack();
        }
    }
    public virtual void Attack()
    {
        InitAttack();
    }
    public void InitAttack()
    {
        attackHitBox.enabled = true;
        timeUntilAttackEnd = Time.time + attackTime;
        attacking = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<IAttackableScript>() != null)
        {
            other.GetComponent<IAttackableScript>().OnAttack(gameObject,0,1, gameObject.transform.forward);
            StopAttack();
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<IAttackableScript>() != null)
        {
            other.GetComponent<IAttackableScript>().OnAttack(gameObject, 0, 1, gameObject.transform.forward);
            StopAttack();
        }
    }
    private void StopAttack()
    {
        attackHitBox.enabled = false;
        attacking = false;
    }

    public void ManualChangeCollider(bool colliderState)
    {
        attackHitBox.enabled = colliderState;
    }
}
