using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DecalScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private DecalProjector projector;
    private RaycastHit hit;
    [SerializeField] Transform playerTransform;
    [SerializeField] Vector3 DecalOffset;
    [SerializeField] Vector3 DecalDrawOffset;
    [SerializeField] float maxDownDist;
    private Vector3 searchPosition;

    void Awake()
    {
        projector = GetComponent<DecalProjector>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        searchPosition = playerTransform.position + DecalOffset; 
        
        if (Physics.Raycast(searchPosition, Vector3.down, out hit, maxDownDist)) {
            projector.transform.position = hit.point + DecalDrawOffset;
            //projector.drawDistance = hit.distance + 0.1f;
            //projector.pivot = Vector3.forward * (hit.distance / 2);
        };
    }
}
