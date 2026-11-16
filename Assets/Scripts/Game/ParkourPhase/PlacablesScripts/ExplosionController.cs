using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]List<ParticleSystem> ExplosionSystems;
    bool hasExploded = false;
    public void OnEnable()
    {
        foreach (var system in ExplosionSystems)
        {
            system.Stop();
        }
    }

    public void Start()
    {
        foreach (var system in ExplosionSystems)
        {
            system.Stop();
        }

        hasExploded = false;
    }
    public void Explode()
    {
        if (hasExploded) { return; }
        foreach (var system in ExplosionSystems)
        { 
            system.gameObject.SetActive(true);
            system.Play();
        }
        hasExploded = true;
    }
}
