using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerCastSpriteAnimator : MonoBehaviour
{
    [Header("Cast Sprites (1~5 순서대로)")]
    public Sprite[] castSprites;

    [Header("FPS")]
    public float fps = 10f;

    private SpriteRenderer sr;
    private Coroutine loopRoutine;
    private Sprite defaultSprite;

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        defaultSprite = sr.sprite;
    }

    public void StartCast()
    {
        Debug.Log("[PlayerCastSpriteAnimator] StartCast()");
        if (loopRoutine != null) return;
        loopRoutine = StartCoroutine(PlayLoop());
    }

    public void StopCast()
    {
        if (loopRoutine != null)
        {
            StopCoroutine(loopRoutine);
            loopRoutine = null;
        }
        sr.sprite = defaultSprite;
    }


    private IEnumerator PlayLoop()
    {
        int index = 0;
        float delay = 1f / Mathf.Max(0.01f, fps);

        while (true)
        {
            if (castSprites == null || castSprites.Length == 0)
                yield break;

            sr.sprite = castSprites[index];
            index = (index + 1) % castSprites.Length;

            yield return new WaitForSeconds(delay);
        }
    }
}
