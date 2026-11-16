using UnityEngine;

public class TargetableScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] string targetName;
    [SerializeField] GameObject myObject;
    void Start()
    {
        AIManager.Instance.RegisterItem(myObject, targetName);
    }
    private void OnDisable()
    {
        AIManager.Instance.UnregisterItem(myObject, targetName);
    }
}
