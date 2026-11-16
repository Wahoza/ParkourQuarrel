using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines.Interpolators;

public class LocalToNetworkObjectMover : MonoBehaviour
{
    [SerializeField] Transform goalTransform;
    [SerializeField] List<GameObject> baseModels;
    [SerializeField] List<Collider> baseColliders;
    [SerializeField] List<MonoBehaviour> baseScripts;


    
    [SerializeField] GameObject objectToCreate;

    [SerializeField] [Range(min:0,max:1)] float lerpStrength;
    private GameObject myObject;
    private bool isClient = false;
    private void OnEnable()
    {
        objectToCreate.SetActive(false);

        if (PlayerInteractionManager.Instance == null)
        {
            return;
        }
        if (!PlayerInteractionManager.Instance.IsHost)
        {
            myObject = Instantiate(objectToCreate);
            myObject.transform.parent = null;
            myObject.transform.position = goalTransform.position;

            myObject.SetActive(true);

            EnableInitObject(false);

            isClient = true;
        }
        else {
            objectToCreate.GetComponent<Rigidbody>().isKinematic = true;
        }
    }
    private void FixedUpdate()
    {

        if (isClient)
        {
            //myObject.transform.localPosition = Vector3.Lerp(myObject.transform.localPosition, goalTransform.localPosition, lerpStrength);
            myObject.transform.position = Vector3.Slerp(myObject.transform.position, new Vector3(goalTransform.position.x,myObject.transform.position.y, goalTransform.position.z), lerpStrength);
            myObject.transform.eulerAngles = new Vector3 (myObject.transform.eulerAngles.x , Mathf.Lerp(myObject.transform.eulerAngles.y, goalTransform.eulerAngles.y, lerpStrength), myObject.transform.eulerAngles.z);//Vector3.Lerp(myObject.transform.eulerAngles, goalTransform.eulerAngles, lerpStrength);
        }
    }
    private void OnDisable()
    {
        if (isClient)
        {
            EnableInitObject(true);
        }

        if(myObject != null)
        {
            Destroy(myObject);
        }
    }
    private void EnableInitObject(bool enabled)
    {
        foreach (GameObject go in baseModels)
        {
            go.SetActive(enabled);
        }
        foreach (Collider col in baseColliders)
        {
            col.enabled = (enabled);
        }
        foreach (MonoBehaviour behaviour in baseScripts)
        {
            behaviour.enabled = (enabled);
        }
    }
}
