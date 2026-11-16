using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiStatsScript : MonoBehaviour
{
    [SerializeField] Image NameBackground;
    [SerializeField] RawImage FaceBackground;
    [SerializeField] RawImage Face;
    [SerializeField] TextMeshProUGUI playerName;
    [SerializeField] GameObject HealthIcon;
    [SerializeField] Transform HealthHolder;
    [SerializeField] GameObject ScribbleOut;

    bool isOnline = false;
    public void SetMaterialColor(Material mat)
    {
        Color color = mat.color;
        color.a = 0.8f;
        FaceBackground.color = color;
        NameBackground.color = color;
    }
    public void SetHealthLocal(int health)
    {
        int currentHealthCount = HealthHolder.childCount;
        for (int i = currentHealthCount - 1; i >= 0; i--)
        {
            Destroy(HealthHolder.GetChild(i).gameObject);

        }

        for (int i = 0; i < health; i++)
        {
            GameObject healthItem = Instantiate(HealthIcon);
            healthItem.transform.SetParent(HealthHolder, false);
            healthItem.SetActive(true);
        }
        if (health <= 0)
        {
            ScribbleOut.SetActive(true);
        }
        else
        {
            ScribbleOut.SetActive(false);
        }
    }
    public void SetHealth(int health)
    {
        if (isOnline)
        {
            PlayerInteractionManager.Instance.RequestHpChange(health);
        }

        SetHealthLocal(health);
    }
    public void SetFace(Texture faceTexture)
    {
        Face.texture = faceTexture;
    }
    public void SetPlayerName(string name)
    {
        playerName.text = name;
    }
    public void SetColor(Color color)
    {
        color.a = 0.8f;

        FaceBackground.color = color;
        NameBackground.color = color;
    }
    public void InitializePlayer(PlayerConfig pc)
    {
        SetMaterialColor(pc.PlayerMaterial);
        SetPlayerName("P" + (pc.PlayerIndex +1).ToString());
        SetFace(pc.PlayerFace);
    }
    public void SetIsOnline (bool online)
    {
        isOnline = online;
    }
}
