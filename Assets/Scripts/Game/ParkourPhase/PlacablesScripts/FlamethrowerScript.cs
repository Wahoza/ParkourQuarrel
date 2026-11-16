using System.Collections.Generic;
using UnityEngine;

public class FlamethrowerScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] float fireRate;
    [SerializeField] Transform fireHolder;
    [SerializeField] Transform fireEmitter;
    [SerializeField] Transform fireTarget;

    private List<GameObject> fireItems;
    private int currentItem = 0;
    private float nextFireTime = 0;
    void Start()
    {
        fireItems = new List<GameObject>();
        for (int i = 0; i < fireHolder.childCount; i++)
        {
            fireItems.Add(fireHolder.GetChild(i).gameObject);
        }
            
    }

    // Update is called once per frame
    void Update()
    {
        if (nextFireTime < Time.time)
        {
            nextFireTime = Time.time + 1/fireRate;
            currentItem++;

            if (currentItem > fireItems.Count - 1)
            {
                currentItem = 0;
            }


            fireItems[currentItem].SetActive(true);
            fireItems[currentItem].transform.position = fireEmitter.position;
            //fireItems[currentItem].transform.rotation = fireEmitter.rotation;
            FireScript fs = fireItems[currentItem].GetComponent<FireScript>();
            fs.ResetFire();
            fs.SetMoveDir((fireTarget.position - fireEmitter.position).normalized);
        }
    }
}
