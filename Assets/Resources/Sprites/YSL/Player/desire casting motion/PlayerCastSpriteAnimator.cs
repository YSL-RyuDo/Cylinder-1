using UnityEngine;
using System.Collections;

public class PlayerCastSpriteAnimator : MonoBehaviour
{
    [Header("Renderer (비우면 자동 탐색)")]
    [SerializeField] private SpriteRenderer targetRenderer;

    [Header("Standing(기본) 스프라이트 (비우면 시작 스프라이트 사용)")]
    [SerializeField] private Sprite standingSprite;

    [Header("Pre Cast (1회 재생)")]
    public Sprite[] preCastSprites;

    [Header("Cast Loop (반복 재생)")]
    public Sprite[] castLoopSprites;

    [Header("Cast End (1회 재생)")]
    public Sprite[] castEndSprites;

    [Header("공통 FPS (Pre/Loop)")]
    public float fps = 10f;

    [Header("Cast End 전용 FPS 사용 여부")]
    public bool useEndFps = true;
    public float endFps = 6f;

    [Header("Cast End: 특정 프레임 이후 추가 지연")]
    [Tooltip("0부터 시작. 3번째 프레임이면 2")]
    public int endSlowAfterFrameIndex = 2;

    [Tooltip("추가로 더 기다릴 프레임 수")]
    public int endExtraFrames = 2;

    private Coroutine sequenceRoutine;
    private Coroutine loopRoutine;

    private enum CastState { Standing, Pre, Loop, End }
    private CastState state = CastState.Standing;

    private void Awake()
    {
        if (targetRenderer == null)
        {
            targetRenderer = GetComponentInChildren<SpriteRenderer>();
            if (targetRenderer == null)
                targetRenderer = GetComponent<SpriteRenderer>();
        }

        if (targetRenderer == null)
        {
            Debug.LogError("SpriteRenderer를 찾을 수 없습니다.");
            enabled = false;
            return;
        }

        if (standingSprite == null)
            standingSprite = targetRenderer.sprite;
    }

    // ===== 외부 호출 =====

    public void BeginCast()
    {
        if (state == CastState.Loop || state == CastState.Pre) return;

        StopAllCoroutinesSafe();
        sequenceRoutine = StartCoroutine(PreThenLoop());
    }

    public void EndCast()
    {
        if (state == CastState.Standing || state == CastState.End) return;

        StopAllCoroutinesSafe();
        sequenceRoutine = StartCoroutine(PlayEndThenStanding());
    }

    public void SnapToStanding()
    {
        StopAllCoroutinesSafe();
        state = CastState.Standing;
        targetRenderer.sprite = standingSprite;
    }

    // ===== 내부 로직 =====

    private IEnumerator PreThenLoop()
    {
        state = CastState.Pre;
        yield return PlayOnce(preCastSprites, fps);
        StartLoop();
    }

    private void StartLoop()
    {
        state = CastState.Loop;
        loopRoutine = StartCoroutine(PlayLoop());
    }

    private IEnumerator PlayEndThenStanding()
    {
        state = CastState.End;

        float useFpsValue = useEndFps ? endFps : fps;
        yield return PlayEndOnce(useFpsValue);

        state = CastState.Standing;
        targetRenderer.sprite = standingSprite;
    }

    private IEnumerator PlayOnce(Sprite[] sprites, float usedFps)
    {
        if (sprites == null || sprites.Length == 0) yield break;

        float delay = 1f / Mathf.Max(0.01f, usedFps);

        for (int i = 0; i < sprites.Length; i++)
        {
            targetRenderer.sprite = sprites[i];
            yield return new WaitForSeconds(delay);
        }
    }

    private IEnumerator PlayLoop()
    {
        if (castLoopSprites == null || castLoopSprites.Length == 0)
        {
            state = CastState.Standing;
            targetRenderer.sprite = standingSprite;
            yield break;
        }

        float delay = 1f / Mathf.Max(0.01f, fps);
        int index = 0;

        while (true)
        {
            targetRenderer.sprite = castLoopSprites[index];
            index = (index + 1) % castLoopSprites.Length;
            yield return new WaitForSeconds(delay);
        }
    }

    private IEnumerator PlayEndOnce(float usedFps)
    {
        if (castEndSprites == null || castEndSprites.Length == 0) yield break;

        float baseDelay = 1f / Mathf.Max(0.01f, usedFps);

        for (int i = 0; i < castEndSprites.Length; i++)
        {
            targetRenderer.sprite = castEndSprites[i];

            float delay = baseDelay;

            if (i == endSlowAfterFrameIndex)
                delay += baseDelay * Mathf.Max(0, endExtraFrames);

            yield return new WaitForSeconds(delay);
        }
    }

    private void StopAllCoroutinesSafe()
    {
        if (sequenceRoutine != null)
        {
            StopCoroutine(sequenceRoutine);
            sequenceRoutine = null;
        }

        if (loopRoutine != null)
        {
            StopCoroutine(loopRoutine);
            loopRoutine = null;
        }
    }

    // ===== 퍼즐 종료 대기용 =====
    public float GetEndDuration()
    {
        if (castEndSprites == null || castEndSprites.Length == 0)
            return 0.2f;

        float usedFps = useEndFps ? endFps : fps;
        float baseDelay = 1f / Mathf.Max(0.01f, usedFps);

        float total = castEndSprites.Length * baseDelay;
        total += baseDelay * Mathf.Max(0, endExtraFrames);

        return total;
    }
}
