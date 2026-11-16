using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class SpinnyScript : MonoBehaviour
{
    [SerializeField] float rotationSpeed;
    [SerializeField] Vector3 rotationAxes;
    [SerializeField] List<Transform> SpinnyObjects;

    [SerializeField] bool localized;
    // Update is called once per frame
    void FixedUpdate()
    {
        if (!localized)
        {
            foreach (Transform t in SpinnyObjects)
            {
                t.RotateAround(transform.position, rotationAxes, rotationSpeed * Time.deltaTime);
            }
        }
        else
        {
            foreach (Transform t in SpinnyObjects)
            {
                t.Rotate(rotationAxes, rotationSpeed * Time.deltaTime);
            }
        }
    }
}
