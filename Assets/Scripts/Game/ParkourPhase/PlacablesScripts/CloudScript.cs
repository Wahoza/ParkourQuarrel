using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class CloudScript : MonoBehaviour
{
    List<Rigidbody> itemsInCloud;
    [SerializeField] float gravityScale;
    void Start()
    {
        itemsInCloud = new List<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        for(int i = itemsInCloud.Count -1; i >= 0; i--) {
            if (itemsInCloud[i] != null)
            {
                itemsInCloud[i].AddForce(-gravityScale * Physics.gravity, ForceMode.Acceleration);
            }
            else
            {
                itemsInCloud.RemoveAt(i);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (itemsInCloud == null) { return; }

        if (other.gameObject.GetComponent<Rigidbody>() != null)
        {
            if (!other.gameObject.GetComponent<Rigidbody>().useGravity) { return; }
            itemsInCloud.Add(other.gameObject.GetComponent<Rigidbody>());

            other.gameObject.GetComponent<Rigidbody>().useGravity = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(itemsInCloud == null) { return; }
        if (other.gameObject.GetComponent<Rigidbody>() != null)
        {
            if (itemsInCloud.Contains(other.GetComponent<Rigidbody>()))
            {
                itemsInCloud.Remove(other.GetComponent<Rigidbody>());
                other.gameObject.GetComponent<Rigidbody>().useGravity = true;
            }

        }
    }
}
