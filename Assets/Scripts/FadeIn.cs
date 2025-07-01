using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeIn : MonoBehaviour
{
    public Image fadeImage;
    public float duration = 2f;
    private Coroutine fadeCoroutine;

    void OnEnable()
    {
        // 알파값 초기화
        if (fadeImage != null)
        {
            Color color = fadeImage.color;
            color.a = 1f;
            fadeImage.color = color;
            fadeImage.gameObject.SetActive(true);
        }

        // 중복 실행 방지
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(Fadein());
    }

    IEnumerator Fadein()
    {
        float time = 0f;
        Color color = fadeImage.color;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            if (t < 0.5f)
            {
                color.a = 1f; // 처음 절반은 불투명 유지
            }
            else
            {
                float fadeT = (t - 0.5f) / 0.5f;
                float eased = 1f - (1f - fadeT) * (1f - fadeT); // EaseOutQuad
                color.a = Mathf.Lerp(1f, 0f, eased);
            }

            fadeImage.color = color;
            yield return null;
        }

        fadeImage.gameObject.SetActive(false);
    }
}
