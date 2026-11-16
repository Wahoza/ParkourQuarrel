using UnityEngine;
using UnityEngine.UIElements;

public class PlayerParticles : MonoBehaviour
{
    [SerializeField] GameObject JumpParticles;
    [SerializeField] GameObject LandParticles;
    [SerializeField] GameObject WaterParticles;
    [SerializeField] GameObject DeathParticles;

    [SerializeField] float landParticlesCooldown;
    [SerializeField] float waterParticleCooldown;

    public float landTimer;
    public float waterTimer;

    AudioManager audioManager;
    public void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
    private enum ParticleType
    {
        Jump = 0,
        Land = 1,
        LandWater = 2,
        Die = 3,
    }
    public virtual void SetColor(Color color)
    {
        ParticleSystem jumpPs = JumpParticles.GetComponent<ParticleSystem>();
        ParticleSystem landPs = LandParticles.GetComponent<ParticleSystem>();
        var jumpMainModule = jumpPs.main; // Get the MainModule
        var landMainModule = landPs.main; // Get the MainModule

        jumpMainModule.startColor = color;
        landMainModule.startColor = color;
    }
    public virtual void Jump(Vector3 position) {
        if(!gameObject.activeSelf) { return; }
        JumpParticles.transform.position = position;
        JumpParticles.SetActive(true);
        JumpParticles.GetComponent<ParticleSystem>().Play();

        audioManager.PlaySFX(audioManager.jump);
    }
    public virtual void Land(Vector3 position) {
        if (!gameObject.activeSelf) { return; }
        if(landTimer > Time.time) { return; }

        LandParticles.transform.position = position;

        LandParticles.SetActive(true);
        LandParticles.GetComponent<ParticleSystem>().Play();

        audioManager.PlaySFX(audioManager.land);

        landTimer = Time.time + landParticlesCooldown; //prevents excessive particles
    }
    public virtual void LandWater(Vector3 position) {
        if (!gameObject.activeSelf) { return; }
        if (waterTimer > Time.time) { return; }

        WaterParticles.transform.position = position;

        WaterParticles.SetActive(true);
        WaterParticles.GetComponent<ParticleSystem>().Play();

        audioManager.PlaySFX(audioManager.splash);
        waterTimer = Time.time + waterParticleCooldown; //prevents excessive particles
    }
    public virtual void Die(Vector3 position) {
        if (!gameObject.activeSelf) { return; }

        DeathParticles.transform.position = position;

        DeathParticles.SetActive(true);
        DeathParticles.GetComponent<ParticleSystem>().Play();
    }
    public void Respawn()
    {
        JumpParticles.GetComponent<ParticleSystem>().Stop();
        LandParticles.GetComponent<ParticleSystem>().Stop();
        WaterParticles.GetComponent<ParticleSystem>().Stop();
        DeathParticles.GetComponent<ParticleSystem>().Stop();
    }
    public void DoParticles(Vector3 pos, int type)
    {
        ParticleType _type = (ParticleType)type;

        if (true)
        {
            switch (_type)
            {
                case ParticleType.Jump:
                    Jump(pos);
                    return;
                case ParticleType.Land:
                    Land(pos);
                    return;
                case ParticleType.LandWater:
                    LandWater(pos);
                    return;
                case ParticleType.Die:
                    Die(pos);
                    return;
            }
        }
    }
}
