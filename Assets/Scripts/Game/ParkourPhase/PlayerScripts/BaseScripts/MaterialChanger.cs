using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MaterialChanger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] List<SkinnedMeshRenderer> skinnedMeshes;
    [SerializeField] List<MeshRenderer> meshes;
    [SerializeField] List<Material> initMaterials;
    [SerializeField] Texture2D face;
    [SerializeField] DecalProjector feetProjector;
    [SerializeField] SpriteRenderer faceSprite;
    [SerializeField] PlayerParticles playerParticles;


    void Awake()
    {
        //ApplyInitialMaterials();
    }
    private void ApplyInitialMaterials()
    {
        foreach (SkinnedMeshRenderer item in skinnedMeshes)
        {
            Material[] materials = item.materials;
            for (int i = 0; i < initMaterials.Count && i < materials.Length; i++)
            {
                materials[i] = initMaterials[i];
            }
            item.materials = materials; // Reassign the modified array
        }

        foreach (MeshRenderer item in meshes)
        {
            Material[] materials = item.materials;
            for (int i = 0; i < initMaterials.Count && i < materials.Length; i++)
            {
                materials[i] = initMaterials[i];
            }
            item.materials = materials; // Reassign the modified array
        }
    }

    public void SetMaterial(Material mat, int slot)
    {
        slot = 0;
        foreach (SkinnedMeshRenderer item in skinnedMeshes)
        {
            Material[] materials = item.materials;

            if (item.materials.Length > slot)
            {
                materials[slot] = mat;
                item.materials = materials;
            }
        }
        foreach (MeshRenderer item in meshes)
        {
            Material[] materials = item.materials;

            if (item.materials.Length > slot)
            {
                materials[slot] = mat;

                item.materials = materials;
            }
        }

        if(slot == 0){
            feetProjector.material = new Material(feetProjector.material);
            feetProjector.material.color = mat.color;
            playerParticles.SetColor(mat.color);
        }
    }
    public void SetFace(Texture2D faceTexture)
    {
        face = faceTexture;

        Sprite newSprite = Sprite.Create(
                face,
                new Rect(0, 0, face.width, face.height),
                new Vector2(0.5f, 0.5f), // Pivot at the center
                faceTexture.width
        );
        faceSprite.sprite = newSprite;
    }

    public void SetFromPlayerConfig(PlayerConfig playerConfig)
    {
        SetMaterial(playerConfig.PlayerMaterial, 0);
        SetFace(playerConfig.PlayerFace);
    }
}
