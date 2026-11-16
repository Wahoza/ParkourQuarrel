using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("Source")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("Clip Player")]
    [SerializeField] public AudioClip music;
    [SerializeField] public AudioClip jump;
    [SerializeField] public AudioClip land;
    [SerializeField] public AudioClip damage;
    [SerializeField] public AudioClip death;

    [Header("Clip Objects")]
    [Header("Clip Goal")]
    [SerializeField] public AudioClip goalAudio;

    [Header("Clip Bomb")]
    [SerializeField] public AudioClip bombTriggerDown;
    [SerializeField] public AudioClip bombExplode;

    [Header("Clip Cannon")]
    [SerializeField] public AudioClip fireCannon;

    [Header("Bread")]
    [SerializeField] public AudioClip breadEat;

    [Header("WaterSound")]
    [SerializeField] public AudioClip splash;

    [Header("UI Sounds")]
    [SerializeField] public AudioClip handConfirm;
    [SerializeField] public AudioClip handDecline;
    [SerializeField] public AudioClip swapHeld;
    [SerializeField] public AudioClip countdownBeep;
    [SerializeField] public AudioClip countdownFinalBeep;
    private void Start()
    {
        musicSource.clip = music;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
}
