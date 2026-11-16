using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeechScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] float showDuration;
    [SerializeField] GameObject speachBubble;
    [SerializeField] Image speachBubbleImage;
    [SerializeField] Image speechHolderImage;
    [SerializeField] List<Sprite> speeches;
    [SerializeField] float speechBubbleGrowthDuration;
    [SerializeField] float maxSize;

    [SerializeField] float despawnDuration;
    private float despawnTimer;

    private bool isSpeaking;
    private float speakTimer;
    private float speechBubbleGrowth;
    private Color defaultColor = new Color(1,1,1);
    private float currentAlpha;

    private bool online = false;
    // Update is called once per frame
    void Update()
    {
        if (isSpeaking)
        {
            if (speachBubble.transform.localScale.magnitude < maxSize)
            {
                speechBubbleGrowth += Time.deltaTime / speechBubbleGrowthDuration;
                speachBubble.transform.localScale = Vector3.one * speechBubbleGrowth;
            }
            if (despawnTimer < Time.time) {
                currentAlpha -= Time.deltaTime / despawnDuration;
                defaultColor.a = Mathf.Clamp01(currentAlpha);

                speachBubbleImage.color = defaultColor;
                speechHolderImage.color = defaultColor;
            }
            if (speakTimer < Time.time)
            {
                speachBubble.SetActive(false);
                isSpeaking = false;
            }
        }
    }
    public void StartSpeak(Vector2 inputDir)
    {
        if (online)
        {
            StartSpeakOnline(inputDir);
        }
        else
        {
            StartSpeakLocal(inputDir);
        }
    }

    public void SetIsOnline(bool isOnline)
    {
        online = isOnline;
    }

    private void StartSpeakLocal(Vector2 inputDir){
        Vector2 input = inputDir;

        input.x = Mathf.Abs(input.x) > Mathf.Abs(input.y) ? input.x : 0;
        input.y = Mathf.Abs(input.y) >= Mathf.Abs(input.x) ? input.y : 0; //>= here so input doesnt reach 0,0


        input.x = input.x > 0 ? 1 : input.x < 0 ? -1 : 0;
        input.y = input.y > 0 ? 1 : input.y < 0 ? -1 : 0;

        int selection = (int)input.y + (1 * (input.y == 0 ? 0 : 1)) + (int)input.x + (2 * (input.x == 0 ? 0 : 1));
        Debug.Log("speaking");

        Speak(selection);
    }

    private void StartSpeakOnline(Vector2 inputDir)
    {
        Vector2 input = inputDir;

        input.x = Mathf.Abs(input.x) > Mathf.Abs(input.y) ? input.x : 0;
        input.y = Mathf.Abs(input.y) >= Mathf.Abs(input.x) ? input.y : 0; //>= here so input doesnt reach 0,0


        input.x = input.x > 0 ? 1 : input.x < 0 ? -1 : 0;
        input.y = input.y > 0 ? 1 : input.y < 0 ? -1 : 0;

        int selection = (int)input.y + (1 * (input.y == 0 ? 0 : 1)) + (int)input.x + (2 * (input.x == 0 ? 0 : 1));

        Speak(selection);

        PlayerInteractionManager.Instance.RequestPlayerSpeak(selection);
    }

    public void Speak(int emoteId)
    {
        if (speeches.Count > emoteId)
        {
            speechHolderImage.sprite = speeches[emoteId];
        }


        isSpeaking = true;
        speachBubble.SetActive(true);

        defaultColor.a = 1;
        currentAlpha = 1;
        speechHolderImage.color = defaultColor;
        speachBubbleImage.color = defaultColor;

        speakTimer = Time.time + showDuration + despawnDuration;
        despawnTimer = Time.time + showDuration;

        speechBubbleGrowth = 0;
        speachBubble.transform.localScale = Vector3.zero;
    }
}
