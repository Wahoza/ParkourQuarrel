using System.Collections.Generic;
using UnityEngine;

public class WallScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    List<GameObject> bricks;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnEnable()
    {
        GetBricks();
        foreach (var item in bricks)
        {
            item.GetComponent<PhaseBehavior>()?.StartParkourPhase();
        }
    }
    private void OnDisable()
    {
        GetBricks();
        foreach (var item in bricks) {
            item.GetComponent<PhaseBehavior>()?.StartPlacingPhase();
        }
    }

    void GetBricks()
    {
        bricks = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            bricks.Add(transform.GetChild(i).gameObject);
        }
    }
}
