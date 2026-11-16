using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class AIManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static AIManager Instance { get; private set; }
    private Dictionary<string, List<GameObject>> targets;
    private List<GameObject> players;
    private List<IAIContollerScript> AIControllers;
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("Trying to create new instance");
        }
        else
        {
            Instance = this;

            targets = new Dictionary<string, List<GameObject>>();
            players = new List<GameObject>();
            AIControllers = new List<IAIContollerScript>();
        }
    }
    public void RegisterItem(GameObject item, string name)
    {
        if (!targets.ContainsKey(name))
        {
            List<GameObject> list = new List<GameObject>();
            list.Add(item);
            targets.Add(name, list);
        }
        else
        {
            targets[name].Add(item);
        }

        foreach(IAIContollerScript controller in AIControllers)
        {
            if (controller.GetWantedTargets().Contains(name))
            {
                controller.AddTarget(item);
            }
        }
    }
    public void UnregisterItem(GameObject item, string name) {
        if (targets.ContainsKey(name))
        {
            targets[name].Remove(item);

            foreach(IAIContollerScript controller in AIControllers)
            {
                if (controller.GetWantedTargets().Contains(name))
                {
                    controller.AddTarget(item);
                }
            }
        }
    }
    public void RegisterPlayer(GameObject player)
    {
        if (!players.Contains(player))
        {
            players.Add(player);
        }
    }
    public void UnregisterPlayer(GameObject player) {
        if (players.Contains(player))
        {
            players.Remove(player);
        }
    }
    public void ClearItems()
    {
        targets.Clear(); 
    }
    public void RegisterController(IAIContollerScript script)
    {
        if (!AIControllers.Contains(script))
        {
            AIControllers.Add(script);

            foreach(string name in script.GetWantedTargets()) {
                if (targets.ContainsKey(name))
                {
                    script.AddTargets(targets[name]);
                }
            }
        }
    }
    public void UnregisterController(IAIContollerScript script) 
    {
        if (AIControllers.Contains(script))
        {
            AIControllers.Remove(script);
        }
    }
}
