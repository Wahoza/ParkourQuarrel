using System.Collections;
using UnityEngine;

using UnityEngine.SceneManagement;

public class LoadingScript : MonoBehaviour
{
    [SerializeField] GameObject LoadingWindow;
    [SerializeField] Transform LoadingBarTransform;
    [SerializeField] float transitionAnimDuration;
    private float animTransitionTimer;
    private bool isLoading = false;

    int SceneToLoad = 0;
    private void Awake()
    {
        LoadingWindow.SetActive(false);
        Cursor.visible = false;
    }
    public void LoadScene(int sceneID)
    {
        animTransitionTimer = Time.time + transitionAnimDuration;
        LoadingWindow.SetActive(true);

        SceneToLoad = sceneID;

        isLoading = true;

    }
    private void Update()
    {
        if (isLoading)
        {
            if(Time.time > animTransitionTimer)
            {
                StartCoroutine(LoadSceneAsync(SceneToLoad));

                isLoading = false;
            }
        }
    }
    IEnumerator LoadSceneAsync(int sceneID)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneID);

        LoadingWindow.SetActive(true);

        while (!op.isDone)
        {
            float progressVal = Mathf.Clamp01(op.progress / 0.9f);
            Debug.Log(op.progress);
            LoadingBarTransform.localScale = new Vector3(progressVal, 1, 1);

            yield return null;
        }
    }
}
