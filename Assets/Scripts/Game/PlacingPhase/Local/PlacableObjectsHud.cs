using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
public class PlacableObjectsHud : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] Transform HudIconParent;
    List<GameObject> imageComponents;
    void Start()
    {
        imageComponents = new List<GameObject>();
        for (int i = 0; i < HudIconParent.childCount; i++)
        {
            imageComponents.Add(HudIconParent.GetChild(i).gameObject);
        }
    }

    public void SetIcons(List<GameObject> prefabs)
    {
        imageComponents = new List<GameObject>();
        for (int i = 0; i < HudIconParent.childCount; i++)
        {
            imageComponents.Add(HudIconParent.GetChild(i).gameObject);
        }

        int counter = 0;
        foreach (GameObject prefab in prefabs)
        {
            if (counter < imageComponents.Count)
            {
                imageComponents[counter].GetComponent<Image>().sprite = prefab.GetComponent<IconHolderScript>().GetIcon();
            }
            counter++;
        }
    }
}
