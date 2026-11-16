using System.Collections.Generic;
using UnityEngine;

public class IAIContollerScript : MonoBehaviour
{
    [SerializeField] List<string> targetNames;
    [SerializeField] List<GameObject> targets;
    private void Start()
    {
        List<GameObject> targets = new List<GameObject>();
        AIManager.Instance.RegisterController(this);
    }
    public void AddTargets(List<GameObject> newtargets)
    {
        targets.AddRange(newtargets);
    }
    public void AddTarget(GameObject target)
    {
        if (!targets.Contains(target)) 
        {
            targets.Add(target); 
        }  
    }
    public void RemoveTarget(GameObject target)
    {
        if (targets.Contains(target)) 
        {
            targets.Remove(target); 
        }  
    }
    public List<string> GetWantedTargets()
    {
        return targetNames;
    }
    
    private void OnDestroy()
    {
        AIManager.Instance.UnregisterController(this);
    }
}
