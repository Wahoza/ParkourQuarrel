using UnityEngine;

public class OnlinePlayerAttackScript : PlayerAttackScript
{
    [SerializeField] OnlinePlayer player;
    public override void Attack()
    {
        PlayerInteractionManager.Instance.RequestAttackAction(player.ID);
    }
}
