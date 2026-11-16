using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class BombExplosionHit : MonoBehaviour
{
    [SerializeField] SphereCollider hitbox;
    [SerializeField] int damage;
    [SerializeField] float playerKnockback;
    [SerializeField] float explosionForce;
    private List<Collider> explodeds;


    public void StartExplosion()
    {
        explodeds = new List<Collider>();

        TryExplode(Physics.OverlapSphere(transform.position, hitbox.radius));
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (!explodeds.Contains(other))
    //    {
    //        TryExplode(other);
    //        explodeds.Add(other);
    //    }
    //    Debug.Log("Splenis");
    //}
    //private void OnTriggerStay(Collider other)
    //{
    //    if (!explodeds.Contains(other))
    //    {

    //        TryExplode(other);
    //        explodeds.Add(other);
    //    }
    //    Debug.Log("Splenis");
    //}

    private void TryExplode(Collider[] other)
    {
        foreach (Collider c in other)
        {
            if (explodeds.Contains(c))
            {
                continue;
            }
            if (c.GetComponent<AttackablePlayer>() != null && c.GetComponent<PlayerManager>().enabled)
            {
                //Debug.Log("We bring the boom");
                c.GetComponent<AttackablePlayer>().OnAttack(gameObject, damage, playerKnockback, (c.transform.position - transform.parent.position).normalized);
            }
            else
            {
                if (c.GetComponent<Rigidbody>())
                {
                    c.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, GetComponent<SphereCollider>().radius);
                }
            }
            explodeds.Add(c);
        }
    }
}
