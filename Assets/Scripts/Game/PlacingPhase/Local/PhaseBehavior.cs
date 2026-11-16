using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class PhaseBehavior : MonoBehaviour
{
    [SerializeField] List<Collider> gameColliders;
    [SerializeField] List<Collider> placingColliders;
    [SerializeField] List<MonoBehaviour> scripts;
    [SerializeField] List<Rigidbody> nonCinematicRBs;
    [SerializeField] List<Animator> animators;
    [SerializeField] List<GameObject> Models;

    private List<int> initialLayers = new List<int>();
    private void Start()
    {
        StartPlacingPhase();
    }
    public void StartParkourPhase()
    {
        foreach (Collider collider in gameColliders) { 
            collider.enabled = true;
        }
        foreach (MonoBehaviour script in scripts)
        {
            script.enabled = true;
        }
        foreach (Rigidbody rb in nonCinematicRBs)
        {
            rb.isKinematic = false;
        }
        foreach (Animator anim in animators)
        {
            anim.enabled = true;
        }
        foreach (Collider collider in placingColliders)
        {
            collider.enabled = false;
        }

        foreach (GameObject model in Models)
        {
            model.SetActive(true);
        }
    }
    public void StartPlacingPhase()
    {
        foreach (Collider collider in gameColliders)
        {
            collider.enabled = false;
        }
        foreach (MonoBehaviour script in scripts)
        {
            script.enabled = false;
        }
        foreach (Rigidbody rb in nonCinematicRBs)
        {
            rb.isKinematic = true;
        }
        foreach (Animator anim in animators)
        {
            anim.enabled = false;
        }
        foreach (Collider collider in placingColliders)
        {
            collider.enabled = true;
        }

        foreach (GameObject model in Models)
        {
            model.SetActive(true);
        }
    }

    public void StartInitialPlacingPhase()
    {
        StartPlacingPhase();
        Transform[] allChildren = GetComponentsInChildren<Transform>();

        foreach (Transform child in allChildren)
        {
            initialLayers.Add(child.gameObject.layer);
            child.gameObject.layer = 2;
        }
    }

    public void EndInitialPlacingPhase()
    {
        Transform[] allChildren = GetComponentsInChildren<Transform>();

        int counter = 0;
        foreach (Transform child in allChildren)
        {
            if (initialLayers.Count > counter)
            {
                child.gameObject.layer = initialLayers[counter];
            }
            counter++;
        }

        initialLayers.Clear();
    }


    //public void OnDisable()
    //{
    //    Destroy(gameObject);
    //}
}
