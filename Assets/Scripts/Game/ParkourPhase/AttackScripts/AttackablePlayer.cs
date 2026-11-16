using Unity.VisualScripting;
using UnityEngine;

public class AttackablePlayer : IAttackableScript
{
    [SerializeField] PlayerController player;
    [SerializeField] PlayerManager playerStats;
    [SerializeField] float StunDuration;
    [SerializeField] float ImpactStrength;
    [SerializeField] float IFrameDuration;
    [SerializeField] Vector3 bonusForce;

    private AudioManager audioManager;


    private float IFrameTime;

    private void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
    public override void OnAttack(GameObject attacker, int damage, float hitStrength, Vector3 knockBackDir)
    {
        if (!enabled) { return; }

        if (IFrameTime < Time.time && playerStats.currentState == 0)
        {
            IFrameTime = Time.time + IFrameDuration;
            playerStats.DecreaseHP(damage);
            if(player.transform.parent.name != "GrabberPocket" && playerStats.currentState == 0)
            {
                player.StunPlayer(StunDuration, knockBackDir);
            }

            GetComponent<Rigidbody>().linearVelocity = Vector3.zero;

            knockBackDir = new Vector3(knockBackDir.x * bonusForce.x, knockBackDir.y * bonusForce.y,knockBackDir.z * bonusForce.z);
            GetComponent<Rigidbody>().AddForce(knockBackDir * ImpactStrength * hitStrength, ForceMode.Impulse);

            if (damage > 0)
            {
                audioManager.PlaySFX(audioManager.damage);
            }
        }
    }
}
