using UnityEngine;

public class DamageCube : IDamageScript
{
    [SerializeField] int damage;
    [SerializeField] int knockbackStrength;
    private void OnTriggerEnter(Collider other)
    {
        DoDamageInteraction(other.gameObject);
    } 
    private void OnTriggerStay(Collider other)
    {
        DoDamageInteraction(other.gameObject);
    }
    public override void DoDamageInteraction(GameObject _gameObject)
    {
        AttackablePlayer attackablePlayer = _gameObject.GetComponent<AttackablePlayer>();
        if(attackablePlayer != null)
        {
            if (_gameObject.GetComponent<PlayerManager>().enabled)
            {
                attackablePlayer.OnAttack(gameObject, damage, knockbackStrength, (transform.position - _gameObject.transform.position).normalized + Vector3.up);
            }
        }
    }
}
