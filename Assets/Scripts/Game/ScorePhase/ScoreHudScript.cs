using UnityEngine;

public class ScoreHudScript : MonoBehaviour
{
    [SerializeField] GameObject ScoreBoard;
    [SerializeField] Animator ScoreBoardAnimator;
    [SerializeField] float leaveDuration;

    private float TimeSinceDisabled;

    private bool isDisabling = false;
    private bool isDisabled = true;
    public void DisableScoreBoard()
    {
        if (isDisabled || isDisabling)
        {
            return;
        }
        ScoreBoardAnimator.SetTrigger("EndAnim");
        isDisabling = true;
        Debug.Log("HEre");
        //isDisabled = true;
    }
    private void Update()
    {
        if (isDisabled)
        {
            return;
        }

        if (isDisabling)
        {
            TimeSinceDisabled += Time.deltaTime;

            if (TimeSinceDisabled > leaveDuration)
            {
                ScoreBoard.SetActive(false);
                TimeSinceDisabled = 0;
                isDisabled = true;
                isDisabling = false;
            }
        }
    }
    public void EnableScoreBoard()
    {
        ScoreBoard.SetActive(true);
        isDisabled = false;
        isDisabling = false;
    }
}
