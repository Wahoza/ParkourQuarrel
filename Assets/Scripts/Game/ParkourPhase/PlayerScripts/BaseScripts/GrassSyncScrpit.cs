using UnityEngine;

public class GrassSyncScrpit : MonoBehaviour
{

    // Update is called once per frame
    public int PosID = Shader.PropertyToID("_Player1Position");
    public int ImpactID = Shader.PropertyToID("_Player1Impact");

    [SerializeField]    
    Material grassMat;
    [SerializeField]
    float virtualHeight;
    public void SetPlayerID(int id)
    {
        PosID = Shader.PropertyToID($"_Player{id}Position");
        ImpactID = Shader.PropertyToID($"_Player{id}Impact");
    }
    void Update()
    {
        //grassMat.SetVector(PosID, transform.position + Vector3.up * virtualHeight);
        //grassMat.SetFloat(ImpactID, 1);
    }

}
