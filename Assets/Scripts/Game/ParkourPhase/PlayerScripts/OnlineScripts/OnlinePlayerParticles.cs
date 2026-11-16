using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class OnlinePlayerParticles : PlayerParticles
{
    [SerializeField] private OnlinePlayer player;

    enum ParticleType
    {
        Jump = 0,
        Land = 1,
        LandWater = 2,
        Die = 3
    }
    private new void Start()
    {
        base.Start();
    }
    public override void Jump(Vector3 pos)
    {
        
        base.Jump(pos);
        PlayerInteractionManager.Instance.RequestParticleAction(pos, (int)ParticleType.Jump, player.ID);
        
    }    
    public override void Land(Vector3 pos)
    {
        if (landTimer < Time.time)
        {
            base.Land(pos);
            PlayerInteractionManager.Instance.RequestParticleAction(pos, (int)ParticleType.Land, player.ID);
        }
    }    
    public override void LandWater(Vector3 pos)
    {
        if (waterTimer < Time.time)
        {
            base.LandWater(pos);
            PlayerInteractionManager.Instance.RequestParticleAction(pos, (int)ParticleType.LandWater, player.ID);
        }
    }
    public override void Die(Vector3 pos)
    {
        
        base.Die(pos);
        PlayerInteractionManager.Instance.RequestParticleAction(pos, (int)ParticleType.Die, player.ID);

        
    }


    //public void DoParticles(Vector3 pos, int type)
    //{
    //    ParticleType _type = (ParticleType)type;

    //    if (true)
    //    {
    //        switch (_type)
    //        {
    //            case ParticleType.Jump:
    //                base.Jump(pos);
    //                return;
    //            case ParticleType.Land:
    //                base.Land(pos);
    //                return;
    //            case ParticleType.LandWater:
    //                base.LandWater(pos);
    //                return;
    //            case ParticleType.Die:
    //                base.Die(pos);
    //                return;
    //        }
    //    }
    //}
}
