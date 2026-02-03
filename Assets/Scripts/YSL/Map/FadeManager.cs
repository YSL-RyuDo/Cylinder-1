using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance;

    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeOutDuration = 0.5f;
    [SerializeField] private float fadeInDuration = 3f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public IEnumerator FadeOut(Action onComplete = null)
    {
        float t = 0f;
        Color c = fadeImage.color;

        while (t < fadeOutDuration)
        {
            t += Time.unscaledDeltaTime;
            c.a = Mathf.Clamp01(t / fadeOutDuration);
            fadeImage.color = c;
            yield return null;
        }

        c.a = 1f;
        fadeImage.color = c;
        onComplete?.Invoke();
    }


    public IEnumerator FadeIn(Action onComplete = null)
    {
        Debug.Log("페이드인");

        Color c = fadeImage.color;

        c.a = 1f;
        fadeImage.color = c;

        float holdTime = 2.0f;
        float t = 0f;

        while (t < holdTime)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        float fadeTime = 1.0f;
        t = 0f;

        while (t < fadeTime)
        {
            t += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(1f, 0f, t / fadeTime);
            fadeImage.color = c;
            yield return null;
        }

        c.a = 0f;
        fadeImage.color = c;

        onComplete?.Invoke();
    }

    public void FadeOutImmediate()
    {
        Color c = fadeImage.color;
        c.a = 1f;
        fadeImage.color = c;
    }

    public void FadeInImmediate()
    {
        Color c = fadeImage.color;
        c.a = 0f;
        fadeImage.color = c;
    }
}
