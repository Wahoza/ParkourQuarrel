using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlacableObjectsThisScene : MonoBehaviour
{
    [SerializeField] List<GameObject> placeableObjects;
    [SerializeField] List<GameObject> placeableObjectsOnline;
    private List<GameObject> placablesGenerated;
    private List<GameObject> placablesGeneratedOnline;
    private List<int> placablesGeneratedIDs;
    public void Awake()
    {
        placablesGenerated = new List<GameObject>();
        placablesGeneratedOnline = new List<GameObject>();
        placablesGeneratedIDs = new List<int>();
    }
    public void GenerateRandomPlacables(int count)
    {
        //placablesGenerated = placeableObjects.OrderBy(x => UnityEngine.Random.Range(0f, 1f)).Take(count).ToList();
        placablesGenerated.Clear();
        placablesGeneratedIDs.Clear();

        for (int i = 0; i < count; i++)
        {
            int num = Random.Range(0, placeableObjects.Count - 1);

            while (placablesGeneratedIDs.Contains(num) && count != placablesGeneratedIDs.Count - 3) //I dont want to perma search, and i think 3 is a good buffer :3
            {
                num = Random.Range(0, placeableObjects.Count - 1);
            }

            if(placeableObjectsOnline.Count == placeableObjects.Count)
            {
                placablesGeneratedOnline.Add(placeableObjectsOnline[num]);
            }

            placablesGenerated.Add(placeableObjects[num]);
            placablesGeneratedIDs.Add(num);
        }
    }   
    public void SetPlacables(List<GameObject> placables)
    {
        placablesGenerated = placables;
    }
    public void ClearPlacables()
    {
        placablesGenerated.Clear();
        placablesGeneratedOnline.Clear();
    }
    public void AddSinglePlacable(int id)
    {
        placablesGenerated.Add(placeableObjects[id]);
    }
    public List<GameObject> GetAllPlacables()
    {
        return placeableObjects;
    }
    public List<GameObject> GetPlaceables()
    {
        return placablesGenerated;
    }    
    public List<GameObject> GetPlaceablesOnline()
    {
        return placablesGeneratedOnline;
    }
    public List<int> GetPlacableIDs()
    {
        return placablesGeneratedIDs;
    }

    public GameObject GetPlacableFromID(int id)
    {
        return placeableObjects[id];
    }    
    public GameObject GetPlacableOnlineFromID(int id)
    {
        return placeableObjectsOnline[id];
    }
}

