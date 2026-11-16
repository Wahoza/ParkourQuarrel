using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ResetShaders : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // Resets all Shaders at start bcz idk 
    [SerializeField] List<Material> WallMaterials;
    [SerializeField] List<Material> GrassMats;
    [SerializeField] int playerCount;
    void Awake()
    {
        for (int i = 0; i<=playerCount - 1; i++) {
            foreach (Material mat in WallMaterials)
            {
                mat.SetFloat($"_SizeP{i.ToString()}", 0);
            }

            foreach (Material mat in GrassMats)
            {
                mat.SetFloat($"Player{i.ToString()}Impact", 0);
            }
        }

    }
}
