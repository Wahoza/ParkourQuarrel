using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviorScript : MonoBehaviour
{
    public List<Transform> players;
    private Vector3 AveragePos;
    [SerializeField] float FollowSpeed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        AveragePos = Vector3.zero;
        foreach (Transform t in players)
        {
            AveragePos += t.position;
        }
        AveragePos /= players.Count;
        Vector3 GoalPos = transform.position;
        GoalPos.x = AveragePos.x;
        transform.position = Vector3.Slerp(transform.position, GoalPos, FollowSpeed * Time.deltaTime);
    }
    public void AddPlayer(Transform player)
    {
        if (!players.Contains(player))
        {
            players.Add(player);
        }
    }
}
