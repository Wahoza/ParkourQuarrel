using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
public class PlayerManager : MonoBehaviour
{
    [SerializeField] public int StartHP;
    [SerializeField] int MaxHP;
    public int CurrentHp = 1;

    [SerializeField] GameObject graveStone;
    [SerializeField] float maxRaydownDist;
    [SerializeField] LayerMask winLayers;
    [SerializeField] GrabberScript grabber;
    [SerializeField] public GameObject playerParticles;
    [SerializeField] GameObject playerDecal;
    [Header("CamStuff")]
    [SerializeField] float playerRadius;
    [SerializeField] float playerVal;
    private UiStatsScript myUIStats;
    private ScoreItemScript myScoreItem;
    private AudioManager audioManager;
    private CinemachineTargetGroup cameraTargetGroup;
    private GameObject playerPointer;
    public int ID {  get; private set; }
    public enum PlayerState
    {
        Alive = 0,
        Dead = 1,
        Finished = 2
    }
    public PlayerState currentState = PlayerState.Alive;
    void Start()
    {
        CurrentHp = StartHP;
        IncreaseScore(0, 5);

        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
//      AIManager.Instance.RegisterPlayer(gameObject);
    }
    public void SetID(int id)
    {
        ID = id;
    }
    public void IncreaseHP(int HP)
    {
        CurrentHp += HP;
        if (CurrentHp > MaxHP)
        {
            CurrentHp = MaxHP;
        }
        myUIStats.SetHealth(CurrentHp);
    }
    public void DecreaseHP(int HP)
    {
        CurrentHp -= HP;
        if (myUIStats != null)
        {
            myUIStats.SetHealth(CurrentHp);
        }
        if (CurrentHp <= 0 && currentState != PlayerState.Dead)
        {
            Die();
        }
    }

    public void SetHP(int HP)
    {
        CurrentHp = HP;
        if (myUIStats != null)
        {
            myUIStats.SetHealth(CurrentHp);
        }

        if (CurrentHp <= 0 && currentState != PlayerState.Dead)
        {
            Die();
        }
    }
    private void Die() 
    {
        graveStone.SetActive(true);
        
        gameObject.GetComponent<PlayerController>().Die();
        gameObject.GetComponent<GrabbablePlayer>().ForceEscape();
        //graveStone.transform.position = transform.position;
        //graveStone.transform.localScale = transform.localScale;

        //Physics.Raycast(graveStone.transform.position, Vector3.down, out RaycastHit hit, maxRaydownDist);
        //graveStone.transform.position = hit.point;

        audioManager.PlaySFX(audioManager.death);

        currentState = PlayerState.Dead;

        Debug.Log("I Died :(");

        IncreaseScore(0, 5);
        OnFinished();
    }
    public void SetUIStatItem(UiStatsScript Item)
    {
        myUIStats = Item;
        myUIStats.SetHealthLocal(StartHP);
    }    
    public void SetScoreItem(ScoreItemScript Item)
    {
        if(Item == null) return;
        myScoreItem = Item;
    }
    public void Respawn()
    {
        //MonoBehaviour[] scripts = gameObject.GetComponentsInChildren<MonoBehaviour>(true);
        //foreach (MonoBehaviour script in scripts)
        //{
        //    script.enabled = true;
        //}

        gameObject.GetComponent<CircleSync>()?.SetCircleSize(1);

        CurrentHp = StartHP;

        if (myUIStats != null)
        {
            myUIStats.SetHealth(CurrentHp);
        }

        currentState = PlayerState.Alive;
        if (playerParticles != null)
        {
            playerParticles.SetActive(true);
        }
        if (playerPointer != null)
        {
            playerPointer.SetActive(true);
        }
        if (playerDecal != null)
        {
            playerDecal.SetActive(true);
        }if (graveStone != null)
        {
            graveStone.SetActive(false);
        }

        playerParticles.GetComponent<PlayerParticles>().Respawn();
        //cameraTargetGroup.AddMember(transform, playerVal, playerRadius);
        IncreaseScore(0, 5);
    }
    public void FocusCam()
    {
        cameraTargetGroup.AddMember(transform, playerVal, playerRadius);
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 7)
        {
            //Finish operation, probably will just disable movement and change an enum
            currentState = PlayerState.Finished;

            return;
        }
    }
    public void OnFinished()
    {

        gameObject.GetComponent<CircleSync>()?.SetCircleSize(0);

        grabber.Release();

        if (cameraTargetGroup != null) { 
            cameraTargetGroup.RemoveMember(transform);
        }
        playerParticles.SetActive(false);

        if (playerPointer != null)
        {
            playerPointer.SetActive(false);
        }
        playerDecal.SetActive(false);
        
        
        gameObject.GetComponent<PlayerController>().CanMove(false); //change this later

        if (PlayerInteractionManager.Instance != null)
        {
            PlayerInteractionManager.Instance.RequestDisableVisual();
        }

        GameManager.Instance.CheckFinished();
        //probably Tally Points Here
    }
    public void OnGoalFinished()
    {    
        currentState = PlayerState.Finished;

        gameObject.GetComponent<PlayerController>().Die();

        gameObject.SetActive(false);

        OnFinished();
    }
    public void EnableGrave(bool enabled)
    {
        graveStone.SetActive(enabled);
    }
    public void DisableGameObject()
    {
        gameObject.SetActive(false);

        if (cameraTargetGroup != null)
        {
            cameraTargetGroup.RemoveMember(transform);
        }
    }
    public void EnableGameObject()
    {
        gameObject.SetActive(true);
    }
    public void SetPlayerPointer(GameObject pointer)
    {
        playerPointer = pointer;
    }
    public void IncreaseScore(int score,int placement)
    {
        if (myScoreItem == null) { return; }
        myScoreItem.SetPointsThisRound(score, placement);
    }
    public void TimeToFinish(float time)
    {
        if (myScoreItem == null) { return; }
        myScoreItem.SetTimeThisRound(time);
    }

    private void Update()
    {
        if(transform.position.y < -200)
        {
            Die();
        }
    }

    internal void SetCinemaTargetGroup(CinemachineTargetGroup _cameraTargetGroup)
    {
        cameraTargetGroup = _cameraTargetGroup;
        cameraTargetGroup.AddMember(transform, playerVal , playerRadius);
    }
}
