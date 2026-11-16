using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class CannonScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] float SyncTime; //Animation Starts a bit b4 i want to shot the ball
    [SerializeField] Animator CannonAnimator;
    [SerializeField] List<GameObject> Cannonballs;
    [SerializeField] float ShotSpeed; // More like a frequency
    [SerializeField] Transform Target;
    [SerializeField] float ShotDelay;
    [SerializeField] float InitialShotOffset;
    private float ShotTimer;
    private int currentBall = 0;
    AudioManager audioManager;
    //consider adding a random offset (only when online tho i dont wanna brute force think this one out...)
    void Start()
    {
        currentBall = 0;
        ShotTimer = Time.time + InitialShotOffset;
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ShotTimer <= Time.time) {
            ShotTimer = Time.time + ShotSpeed;

            ShootAnim();
            Invoke(nameof(Shoot), ShotDelay);
        }
    }
    void ShootAnim()
    {
        CannonAnimator.SetTrigger("Shoot");
    }
    void Shoot()
    {
        currentBall++;
        if(currentBall >= Cannonballs.Count)
        {
            currentBall = 0;
        }
        Cannonballs[currentBall].SetActive(true);
        Cannonballs[currentBall].GetComponent<CannonBallScript>().Fire(Target, ShotSpeed);
        audioManager.PlaySFX(audioManager.fireCannon);
    }
}
