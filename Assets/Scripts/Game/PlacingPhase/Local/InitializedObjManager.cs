using System.Collections.Generic;
using UnityEngine;

public class InitializedObjManager: MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    MonoBehaviour[] scripts;
    [SerializeField] bool RBisKinematic;
    void Start()
    {
        scripts = GetComponents<MonoBehaviour>();
        Invoke(nameof(StartDelayed), 0.001f);
    }

    void StartDelayed()
    {
        if(transform.parent.GetComponent<Rigidbody>() != null)
        {
            SetActive(!transform.parent.GetComponent<Rigidbody>().isKinematic);
        }
    }
    void SetActive(bool active)
    {
        foreach (var script in scripts)
        {
            if(script != this)
            {
                script.enabled = active;
            }
        }

        if (GetComponent<Rigidbody>() != null)
        {
            if (!RBisKinematic)
            {
                GetComponent<Rigidbody>().isKinematic = !active;
            }
        }
    }
}
