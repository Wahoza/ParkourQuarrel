using System.Collections.Generic;
using UnityEngine;

public class ChangeLerpBehavior : MonoBehaviour
{
    [SerializeField] List<Rigidbody> rigidbodies;
    private void Start()
    {
        bool lerp = OnlineGameFlowManager.Instance.IsHost;

        foreach (var rigidbody in rigidbodies)
        {
            rigidbody.interpolation = lerp ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None;
        }
    }
}
