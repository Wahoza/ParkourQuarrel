using UnityEngine;

public class SpeedDamage : IDamageScript
{
    [SerializeField] int damage;
    [SerializeField] float hitStrength;
    [SerializeField] float minVelocityMagnitude;
    public override void DoDamageInteraction(GameObject _gameObject) {
        if (_gameObject.GetComponent<AttackablePlayer>() != null && GetComponent<Rigidbody>().linearVelocity.magnitude > minVelocityMagnitude) {
            
            _gameObject.GetComponent<AttackablePlayer>().OnAttack(gameObject, damage, hitStrength, gameObject.GetComponent<Rigidbody>().linearVelocity);

        }
    }
    private void OnTriggerEnter(Collider other)
    {
        DoDamageInteraction(other.gameObject);
    }
}
