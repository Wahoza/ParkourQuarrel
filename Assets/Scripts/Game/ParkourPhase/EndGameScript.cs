using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameScript : MonoBehaviour
{
    [SerializeField] float EndAnimDuration;
    [SerializeField] int EndScene;

    private float EndTimer;
    bool isEnding = false;
    public void StartEnd()
    {
        EndTimer = Time.time + EndAnimDuration; 
        isEnding = true;
    }
    private void Update()
    {
        if (isEnding)
        {
            if (Time.time > EndTimer)
            {
                SceneManager.LoadScene(EndScene);
            }
        }
    }
}
