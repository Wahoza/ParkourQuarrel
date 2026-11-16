using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;
public class PlayerSetupMenuController : MonoBehaviour
{
    private int PlayerIndex;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private GameObject readyPanel;
    [SerializeField] private GameObject menuPanel;

    [Header("MatPreview")]
    [SerializeField] private RawImage MaterialPreview;
    [SerializeField] private List<Material> materials;
    [SerializeField] private GameObject colorTaken;
    private List<int> takenColors;
    private bool currentColorTaken;
    private int colorIndex = 0;

    [Header("FacePreview")]
    [SerializeField] private RawImage FacePreview;
    [SerializeField] private List<Texture2D> faces;

    [Header("ReadySlot")]
    [SerializeField] private RawImage Ready;
    [SerializeField] private GameObject ReadyComponent; 
    
    [Header("LeaveSlot")]
    [SerializeField] private RawImage Leave;
    private int faceIndex = 0;

    private int selectedSlot = 0; //max 2, is to help with switching selection Slots

    private PlayerInputMap controls;

    private float ignoreInputTime = .5f;
    private bool inputIgnored = false;
    private bool inputEnabled = false;

    // Update is called once per frame

    public void SetPlayerIndex(int pi)
    {
        PlayerIndex = pi;
        titleText.SetText("P" + (pi + 1).ToString());
        ignoreInputTime = Time.time + ignoreInputTime;
        controls = new PlayerInputMap();
    }
    private void Start()
    {
        colorTaken.SetActive(false);
        currentColorTaken = false;
        ChangeSelection(Vector2Int.zero);

        takenColors = new List<int>();
        PlayerConfigManager.Instance.JoinSetupMenu(this);
    }
    void Update()
    {
        if (Time.time > ignoreInputTime && !inputIgnored)
        {
            inputEnabled = true;
            inputIgnored = true;
        }
    }

    public void SetColor(Material color)
    {
        if (!inputEnabled) { return; }
        PlayerConfigManager.Instance.SetPlayerColor(PlayerIndex, color);
    }
    public void SetFace(Texture2D Face)
    {
        if (!inputEnabled) { return; }
        PlayerConfigManager.Instance.SetPlayerFace(PlayerIndex, Face);
    }

    public void ReadyPlayer()
    {
        if (!inputEnabled || currentColorTaken) { return; }

        PlayerConfigManager.Instance.TakeColor(colorIndex);
        PlayerConfigManager.Instance.ReadyPlayer(PlayerIndex);

        ReadyComponent.SetActive(true);

        //transform.GetChild(0).gameObject.SetActive(false);
        inputEnabled = false;
        //enabled = false;
    }    
    public void UnReadyPlayer()
    {
        //if (!inputEnabled) { return; }
        inputEnabled = true;
        PlayerConfigManager.Instance.UnReadyPlayer(PlayerIndex);
        
        PlayerConfigManager.Instance.RemoveColor(colorIndex);
        ReadyComponent.gameObject.SetActive(false);
    }
    public void SelectColor(int inputDirection)
    {
        colorIndex += inputDirection;
        if (colorIndex < 0) { colorIndex = materials.Count - 1; }
        if (colorIndex >= materials.Count) { colorIndex = 0; }
        Debug.Log(colorIndex);
        Material selectedMaterial = materials[colorIndex];
        if (selectedMaterial.HasProperty("_Color")) // Check if the material has a color property
        {
            Color matColor = selectedMaterial.GetColor("_Color");
            matColor.a = 0.9f;
            MaterialPreview.color = matColor;

            currentColorTaken = takenColors.Contains(colorIndex);
            colorTaken.SetActive(currentColorTaken);

        }
        else
        {
            Debug.Log("ERROR");
        }
    }
    public void SetColorsTaken(List<int> newTakenColors)
    {
        takenColors = newTakenColors;

        currentColorTaken = takenColors.Contains(colorIndex);
        colorTaken.SetActive(currentColorTaken);
    }
    public void SelectFace(int inputDirection)
    {
        faceIndex += inputDirection;
        if (faceIndex < 0) { faceIndex = faces.Count - 1; }
        if (faceIndex >= faces.Count) { faceIndex = 0; }

        Texture2D selectedFace = faces[faceIndex];
        FacePreview.texture = selectedFace;
    }
    
    public void ChangeSelection(Vector2Int inputDirection)
    {
        if (!inputEnabled) { return; }
        if (Math.Abs(inputDirection.x) > 0)
        {
            switch (selectedSlot)
            {
                case 0:
                    SelectColor(inputDirection.x);
                    return;
                case 1:
                    SelectFace(inputDirection.x);
                    return;
                default:
                    break;
            }
        }


        else if (Math.Abs(inputDirection.y) > 0)
        {
            selectedSlot = selectedSlot - inputDirection.y;
            if (selectedSlot < 0) { selectedSlot = 3; }
            if (selectedSlot > 3) { selectedSlot = 0; }

            switch (selectedSlot)
            {
                case 0:
                    MaterialPreview.transform.GetChild(1).gameObject.SetActive(true);
                    FacePreview.transform.GetChild(1).gameObject.SetActive(false);
                    Ready.transform.GetChild(0).gameObject.SetActive(false);
                    Leave.transform.GetChild(0).gameObject.SetActive(false);

                    return;
                case 1:
                    MaterialPreview.transform.GetChild(1).gameObject.SetActive(false);
                    FacePreview.transform.GetChild(1).gameObject.SetActive(true);
                    Ready.transform.GetChild(0).gameObject.SetActive(false);
                    Leave.transform.GetChild(0).gameObject.SetActive(false);

                    return;
                case 2:
                    MaterialPreview.transform.GetChild(1).gameObject.SetActive(false);
                    FacePreview.transform.GetChild(1).gameObject.SetActive(false);
                    Ready.transform.GetChild(0).gameObject.SetActive(true);
                    Leave.transform.GetChild(0).gameObject.SetActive(false);
                    return;
                case 3:
                    MaterialPreview.transform.GetChild(1).gameObject.SetActive(false);
                    FacePreview.transform.GetChild(1).gameObject.SetActive(false);
                    Ready.transform.GetChild(0).gameObject.SetActive(false);
                    Leave.transform.GetChild(0).gameObject.SetActive(true);
                    return;

            }
        }

    }
    public void SelectButtonDown()
    {
        if (selectedSlot == 2) {
            SetColor(materials[colorIndex]);
            SetFace(faces[faceIndex]);
            ReadyPlayer();
        }
        if (selectedSlot == 3)
        {
            Disconnect();
        }
    }
    public void Disconnect()
    {
        PlayerConfigManager.Instance.LeaveSetupMenu(this, PlayerIndex);

        Destroy(transform.parent.gameObject); //we are using the new setup here
    }
    public void CancelButtonDown()
    {

    }
}