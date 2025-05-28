using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class FadeOut : MonoBehaviour
{
    public Image fadeImage;
    public float duration = 2f;
    void Start()
    {
        StartCoroutine(Fadeout());
    }

    IEnumerator Fadeout()
    {
        float time = 0f;
        Color color = fadeImage.color;
        color.a = 0f; // 초기 알파값을 0으로 설정
        fadeImage.color = color;

        while (time < duration)
        {
            time += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, time / duration);
            fadeImage.color = color;
            yield return null;
        }

    }
}