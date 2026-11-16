using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HandControllerOnline : NetworkBehaviour
{
    [SerializeField] GameObject hand;
    [SerializeField] GameObject handPoint;
    [SerializeField] GameObject pointer;
    [SerializeField] GameObject handGrab;
    [SerializeField] GameObject ObjectSpawner;
    [SerializeField] GameObject Eraser;
    [SerializeField] ParticleSystem placeParticles;
    [SerializeField] Rigidbody rb;
    [SerializeField] Collider handCollider;
    [SerializeField] float spacing;
    [SerializeField] GameObject objectSelectionPointer;

    GameObject PotentialSpawn;
    public Camera HudCamera;

    public int id {  get; private set; }    
    //[SerializeField] GameObject arm;

    List<GameObject> placables; //we set these in the GameManager

    [SerializeField] private float moveSpeed;


    Vector3 moveDir = Vector3.zero;
    Vector3 lookDir = Vector3.zero;

    int currentItem = 0;

    private float cooldown = 0.2f;
    private float CoolDownTimer = 0;

    private bool isLocalHand = false;
    private void Awake()
    {
        placables = new List<GameObject>();
    }
    public enum currentPlacingState
    {
        erasing = -1,
        hovering = 0,
        rotating = 1,
        finished = 2
    }
    private currentPlacingState state = currentPlacingState.finished;
    public int GetCurrentState()
    {
        return (int)state;
    }
    AudioManager audioManager;
    public void StartPlacing()
    {
        Debug.Log("Started PLacing " + GetComponent<HandID>().ID);
        

        transform.localPosition = Vector3.zero + Vector3.left * spacing * id;
        //transform.position = HudCamera.transform.position - Vector3.down* 0.1f;
        GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
        state = currentPlacingState.hovering;

        ObjectSpawner.SetActive(true);
        ObjectSpawner.transform.rotation = Quaternion.Euler(0, 0, 0);
        handPoint.SetActive(false);
        handGrab.SetActive(true);
        handCollider.enabled = true;
        rb.isKinematic = false;

        isLocalHand = GetComponent<HandID>().ID == PlayerInteractionManager.Instance.GetPlayerId();
        Debug.Log("Is my Hand " + isLocalHand);

        GetComponent<HandleHandInputsOnline>().SwitchToDifferentHeldServerRpc(0, id); 
        //SetHeld(0);
        SetCurrentState(0, true);

        CoolDownTimer  = Time.time + cooldown;

        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();

        Debug.Log("StartedPlacing");
        gameObject.SetActive(true);
        placeParticles.Stop();

        SwapHeld(0);
    }
    public void SwitchState(int direction)
    {
        if (audioManager != null && state != currentPlacingState.finished) {
            if (direction > 0) {
                audioManager.PlaySFX(audioManager.handConfirm);
            }
            else
            {
                audioManager.PlaySFX(audioManager.handDecline);
            }
        }
        if(CoolDownTimer > Time.time) { return; }

        if(state == currentPlacingState.finished) {return;}
        
        if(state != currentPlacingState.erasing)
        {
            int nextState = (int)state + direction;

            if (!(nextState > 3 || nextState < 0))
            {
                state += direction;
            }
        }


        GetComponent<HandleHandInputsOnline>().SwitchToStateServerRpc((int)state, GetComponent<HandID>().ID);

        Debug.Log((int)state + " Sent State");

        switch (state)
        {
            case currentPlacingState.erasing:
                RaycastHit hit;
                float maxDistance = 3000;
                if(Physics.Raycast(ObjectSpawner.transform.position, Vector3.down, out hit, maxDistance))
                {
                    GameObject ToBeErased = FindParentWithTag(hit.collider.gameObject, "Spawner");
                    if(ToBeErased != null)
                    {
                        if (isLocalHand)
                        {
                            ToBeErased.GetComponent<PlacedItemOnline>()?.EraseItem();
                            ServerGameManager.Instance.CheckFinishedServerRpc(PlayerInteractionManager.Instance.GetInstanceID());
                        }
                       
                        placeParticles.Play();

                        state = currentPlacingState.finished;
                        Eraser.SetActive(false);
                        handPoint.SetActive(false);
                        handGrab.SetActive(false);
                        handCollider.enabled = false;

                    }
                }


                return;

            case currentPlacingState.hovering:
                handPoint.SetActive(false);
                handGrab.SetActive(true);
                rb.isKinematic = false;
                return;
            case currentPlacingState.rotating:
                //ObjectSpawner.transform.rotation = new Quaternion(0,0,0,1);
                handPoint.SetActive(true);
                handGrab.SetActive(false);
                rb.isKinematic = true;

                return;
            case currentPlacingState.finished:
                handPoint.SetActive(false);
                handGrab.SetActive(false);
                handCollider.enabled =false;

                placeParticles.Play();
                if (isLocalHand)
                {
                    ObjectSpawner.GetComponent<ObjectSpawnerOnline>().SpawnGameObject(placables[currentItem], currentItem);
                    ServerGameManager.Instance.CheckFinishedServerRpc(PlayerInteractionManager.Instance.GetInstanceID());
                }

                gameObject.SetActive(false);

                return;
        }

    }
    public void Initialize(Color color, RectTransform selectableObjectsHolder, bool isLocalPlayer, int handId)
    {
        handPoint.transform.GetChild(1).GetComponent<RawImage>().color = color;
        handGrab.transform.GetChild(0).GetComponent<RawImage>().color = color;

        ObjectSpawner.transform.parent = null;

        id = handId;


        if (isLocalPlayer)
        {
            objectSelectionPointer.transform.GetChild(0).GetComponent<RawImage>().color = color;
            objectSelectionPointer.GetComponent<ItemSelectionPointer>().SetSelectableParent(selectableObjectsHolder);
        }
        else
        {
            objectSelectionPointer.transform.GetChild(0).gameObject.SetActive(false);
            pointer.SetActive(false);
        }

        SetCurrentState(0, false);
        //arm.GetComponent<SpriteRenderer>().color = pc.PlayerMaterial.color;
        //arm.GetComponent<SpriteRenderer>().sortingOrder = pc.PlayerIndex;
    }

    
    public void SetInputVector(Vector2 input)
    {
        moveDir = new Vector3(input.x, 0, input.y);
        lookDir = new Vector3(input.x,0,input.y);
    }

    void FixedUpdate() {

        switch (state)
        {
            case currentPlacingState.erasing:
                rb.linearVelocity = ( moveDir * Time.deltaTime * moveSpeed);
                ObjectSpawner.transform.position = transform.position;

                return;

            case currentPlacingState.hovering:

                if (rb.isKinematic) { return; }
                rb.linearVelocity = (moveDir * Time.deltaTime * moveSpeed);
                ObjectSpawner.transform.position = transform.position;

                return;
            case currentPlacingState.rotating:
                //rb.linearVelocity = Vector3.zero;
                ObjectSpawner.transform.LookAt(ObjectSpawner.transform.position + lookDir);
                pointer.transform.rotation = ObjectSpawner.transform.rotation;
                pointer.transform.Rotate(90, 0, 0, Space.Self);
                return;
        }
        moveDir = Vector3.zero;
    }

    public void SetPlaceableObjectList(List<GameObject> placeablesThisRound)
    {
        Debug.Log(GetComponent<HandID>().ID);

        placables = placeablesThisRound;
    }

    public void SwapHeld(int swapDir)
    {
        if (audioManager != null && state != currentPlacingState.finished)
        {
            audioManager.PlaySFX(audioManager.swapHeld);
        }
        if (state == currentPlacingState.hovering || state == currentPlacingState.erasing) {

            currentItem += swapDir;

            if(currentItem < 0)
            {
                currentItem = placables.Count;
            }

            if (currentItem > placables.Count) {
                currentItem = 0;
            }


            Destroy(PotentialSpawn);

            objectSelectionPointer.GetComponent<ItemSelectionPointer>().SwitchSelection(currentItem);

            if (currentItem == placables.Count)
            {
                state = currentPlacingState.erasing;
                Eraser.SetActive(true);
            }
            else
            {
                state = currentPlacingState.hovering;

                Eraser.SetActive(false);

                PotentialSpawn = Instantiate(placables[currentItem]);
                PotentialSpawn.GetComponent<PhaseBehavior>().StartInitialPlacingPhase();
                //PotentialSpawn.GetComponent<Rigidbody>().isKinematic = true;
                ObjectSpawner.GetComponent<ObjectSpawnerOnline>()?.SetHeldObject(PotentialSpawn);
            }
        }

        if (isLocalHand)
        {
            GetComponent<HandleHandInputsOnline>().SwitchToDifferentHeldServerRpc(GetHeldItem(), GetComponent<HandID>().ID);
        }

    }
    public void SetHeld(int item)
    {
        if (state == currentPlacingState.hovering || state == currentPlacingState.erasing)
        {

            currentItem = item;

            if (currentItem < 0)
            {
                currentItem = placables.Count;
            }

            if (currentItem > placables.Count)
            {
                currentItem = 0;
            }


            Destroy(PotentialSpawn);

            if (currentItem == placables.Count)
            {
                state = currentPlacingState.erasing;
                Eraser.SetActive(true);
            }
            else
            {
                state = currentPlacingState.hovering;

                Eraser.SetActive(false);

                PotentialSpawn = Instantiate(placables[currentItem]);
                PotentialSpawn.GetComponent<PhaseBehavior>().StartInitialPlacingPhase();
                //PotentialSpawn.GetComponent<Rigidbody>().isKinematic = true;
                ObjectSpawner.GetComponent<ObjectSpawnerOnline>()?.SetHeldObject(PotentialSpawn);
            }
        }
    }
    public int GetHeldItem() { return currentItem; }
    public void SetCurrentState(int _newState, bool IsLocalPlayerHand)
    {
        currentPlacingState newState = (currentPlacingState)_newState;

        Debug.Log(newState.ToString() + " Recieved new State " + newState);
        switch (newState)
        {
            case currentPlacingState.hovering:
                handPoint.SetActive(false);
                handGrab.SetActive(true);
                rb.isKinematic = false;
                return;
            case currentPlacingState.rotating:
                //ObjectSpawner.transform.rotation = new Quaternion(0,0,0,1);
                handPoint.SetActive(true);
                handGrab.SetActive(false);
                rb.isKinematic = true;

                return;
            case currentPlacingState.finished:
                handPoint.SetActive(false);
                handGrab.SetActive(false);
                handCollider.enabled = false;

                if (IsLocalPlayerHand)
                {
                    Debug.Log("Im Local");
                    ObjectSpawner.GetComponent<ObjectSpawnerOnline>().SpawnGameObject(placables[currentItem], currentItem);
                    ServerGameManager.Instance.CheckFinishedServerRpc(PlayerInteractionManager.Instance.GetInstanceID());
                }

                gameObject.SetActive(false);
                ObjectSpawner.SetActive(false);
                return;
        }
        state = newState;
    }
    private static GameObject FindParentWithTag(GameObject child, string tag)
    {
        Transform current = child.transform;

        while (current != null)
        {
            if (current.CompareTag(tag))
            {
                return current.gameObject;
            }
            current = current.parent;
        }

        return null; // No parent with the tag found
    }

    public void HideHand()
    {
        handPoint.SetActive(false);
        handGrab.SetActive(false);
    }
}
