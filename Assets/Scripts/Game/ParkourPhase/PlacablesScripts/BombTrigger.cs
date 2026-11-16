using UnityEngine;

public class BombTrigger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] ExplosionController explosion;
    [SerializeField] GameObject mineModel;
    [SerializeField] GameObject explosionHitbox;
    [SerializeField] GameObject mineGameObject;
    [SerializeField] float explosionDelay;
    bool exploded;

    AudioManager audioManager;

    int triggers;
    private void OnEnable()
    {
        exploded = false;
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        triggers = 0;

        explosionHitbox.SetActive(false);
    }
    private void OnTriggerExit(Collider other)
    {
        if (exploded) { return; }
        if (other.gameObject.GetComponent<IAttackableScript>() != null)
        {
            triggers--;
            if(triggers <= 0)
            {
                exploded = true;
                Invoke(nameof(doExplosion), explosionDelay);
            }
            
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(exploded) { return; }
        if(other.gameObject.GetComponent<IAttackableScript>() != null)
        {
            triggers++;
            audioManager.PlaySFX(audioManager.bombTriggerDown);
        }
    }
    void doExplosion()
    {
        audioManager.PlaySFX(audioManager.bombExplode);
        explosion.Explode();
        mineGameObject.SetActive(false);
        explosionHitbox.SetActive(true);
        explosionHitbox.GetComponent<BombExplosionHit>()?.StartExplosion();
        foreach(var col in GetComponents<Collider>())
        {
            col.enabled = false;
        }

        exploded = true;
    }
}
