using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class HandController : MonoBehaviour
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
    //[SerializeField] GameObject arm;

    List<GameObject> Placeables; //we set these in the GameManager

    [SerializeField] private float moveSpeed;


    Vector3 moveDir = Vector3.zero;
    Vector3 lookDir = Vector3.zero;

    int CurrentItem = 0;

    private float cooldown = 0.2f;
    private float CoolDownTimer = 0;

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
    public void StartPlacing(int pID)
    {
        transform.localPosition = Vector3.zero + Vector3.left * spacing * pID;
        //transform.position = HudCamera.transform.position - Vector3.down* 0.1f;
        GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
        state = currentPlacingState.hovering;

        ObjectSpawner.SetActive(true);
        ObjectSpawner.transform.rotation = Quaternion.Euler(0, 0, 0);
        handPoint.SetActive(false);
        handGrab.SetActive(true);
        handCollider.enabled = true;
        rb.isKinematic = false;
        SwapHeld(0);
        
        CoolDownTimer  = Time.time + cooldown;

        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        placeParticles.Stop();
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
                        ToBeErased.GetComponent<PlacedItem>()?.EraseItem();
                        Destroy(ToBeErased);

                        placeParticles.Play();

                        state = currentPlacingState.finished;
                        GameManager.Instance.CheckFinished();
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
                ObjectSpawner.GetComponent<ObjectSpawner>().SpawnGameObject(Placeables[CurrentItem]);
                GameManager.Instance.CheckFinished();
                return;
        }

    }
    public void Initialize(PlayerConfig pc, RectTransform selectableObjectsHolder)
    {
        handPoint.transform.GetChild(1).GetComponent<RawImage>().color = pc.PlayerMaterial.color;
        handGrab.transform.GetChild(0).GetComponent<RawImage>().color = pc.PlayerMaterial.color;

        ObjectSpawner.transform.parent = null;
        
        objectSelectionPointer.transform.GetChild(0).GetComponent<RawImage>().color = pc.PlayerMaterial.color;
        objectSelectionPointer.GetComponent<ItemSelectionPointer>().SetSelectableParent(selectableObjectsHolder);
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

    public void SetPlaceableObjectList(List<GameObject> placeables)
    {
        Placeables = placeables;
    }

    public void SwapHeld(int swapDir)
    {
        if (audioManager != null && state != currentPlacingState.finished)
        {
            audioManager.PlaySFX(audioManager.swapHeld);
        }
        if (state == currentPlacingState.hovering || state == currentPlacingState.erasing) {

            CurrentItem += swapDir;

            if(CurrentItem < 0)
            {
                CurrentItem = Placeables.Count;
            }

            if (CurrentItem > Placeables.Count) {
                CurrentItem = 0;
            }


            Destroy(PotentialSpawn);

            objectSelectionPointer.GetComponent<ItemSelectionPointer>().SwitchSelection(CurrentItem);

            if (CurrentItem == Placeables.Count)
            {
                state = currentPlacingState.erasing;
                Eraser.SetActive(true);
            }
            else
            {
                state = currentPlacingState.hovering;

                Eraser.SetActive(false);

                PotentialSpawn = Instantiate(Placeables[CurrentItem]);
                PotentialSpawn.GetComponent<PhaseBehavior>().StartInitialPlacingPhase();
                //PotentialSpawn.GetComponent<Rigidbody>().isKinematic = true;
                ObjectSpawner.GetComponent<ObjectSpawner>()?.SetHeldObject(PotentialSpawn);
            }
        }
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
}
