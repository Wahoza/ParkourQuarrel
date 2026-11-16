using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class OnlineMaterialChanger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] SkinnedMeshRenderer Feet;
    [SerializeField] List<MeshRenderer> RegularMeshes;
    [SerializeField] Texture2D face;
    [SerializeField] DecalProjector feetProjector;
    [SerializeField] SpriteRenderer faceSprite;
    [SerializeField] OnlinePlayerParticles playerParticles;

    private Material mat;
    private Material feetMat;

    void Awake()
    {
        mat = new Material(RegularMeshes[0].material);

        Feet.material = mat;

        foreach (var mesh in RegularMeshes) 
        { 
            mesh.material = mat; 
        }
        

        if(feetProjector != null)
        {
            feetMat = new Material(feetProjector.material);
            feetProjector.material = feetMat;
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

    public void SetPlayerColor(Color color)
    {
        mat.color = color;

        if(feetMat != null)
        {
            feetMat.color = color;
        }
        if (playerParticles != null)
        {
            playerParticles.SetColor(color);
        }
    }
    public void HideDecal()
    {
        feetProjector.gameObject.SetActive(false);
    }    
    public void ShowDecal()
    {
        feetProjector.gameObject.SetActive(true);
    }
}
