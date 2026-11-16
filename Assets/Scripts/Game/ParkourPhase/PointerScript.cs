using TMPro;
using UnityEngine;

public class PointerScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Camera mainCamera;
    public Transform player;
    [SerializeField] TextMeshProUGUI playerName;
    float speed = 0.1f;
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (player != null) {

            //Debug.Log(player.position);
            //Debug.Log(mainCamera.WorldToScreenPoint(player.position));
            //Debug.Log(transform.position + "myTransform");
            Vector3 playerScreenPosition = mainCamera.WorldToScreenPoint(player.position);
            if ((transform.position - playerScreenPosition).sqrMagnitude > 1)
            {
                transform.position = playerScreenPosition;

                //transform.position = Vector3.Slerp(playerScreenPosition, transform.position,1);
            }
        }
    }

    public void InitializePlayer(PlayerConfig pc, Transform playerTransform, Camera camera) {
        if (playerName != null)
        {
            playerName.text = "P" + (pc.PlayerIndex + 1).ToString();
            playerName.color = pc.PlayerMaterial.color;
        }
        player = playerTransform;
        mainCamera = camera;
    }

    public void SetPlayer(Transform playerTransform)
    {
        player = playerTransform;
    }

    public void SetColor(Color color)
    {
        playerName.color = color;
    }

    public void SetName(string name)
    {
        playerName.text = name;
    }
}
