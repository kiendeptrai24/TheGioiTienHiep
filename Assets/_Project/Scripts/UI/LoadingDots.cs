using UnityEngine;
using TMPro;
using System.Collections;

public class LoadingDots : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI loadingText;

    [Header("Settings")]
    public string baseText = "LOADING";
    public int maxDots = 3;
    public float interval = 0.2f;

    Coroutine loopCoroutine;

    void OnEnable()
    {
        StartLoading();
    }

    void OnDisable()
    {
        StopLoading();
    }

    public void StartLoading()
    {
        if (loadingText == null) return;
        StopLoading();
        loopCoroutine = StartCoroutine(LoadingLoop());
    }

    public void StopLoading()
    {
        if (loopCoroutine != null)
        {
            StopCoroutine(loopCoroutine);
            loopCoroutine = null;
        }
        if (loadingText != null)
            loadingText.text = baseText;
    }

    IEnumerator LoadingLoop()
    {
        int dots = 0;
        while (true)
        {
            dots = (dots + 1) % (maxDots + 1);
            loadingText.text = baseText + (dots == 0 ? "" : " " + new string('.', dots));
            yield return new WaitForSeconds(interval);
        }
    }
}