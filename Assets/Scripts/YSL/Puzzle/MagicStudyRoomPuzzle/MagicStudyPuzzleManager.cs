using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MagicStudyPuzzleManager : MonoBehaviour
{
    [Header("일반 발판 타일 부모(여기 아래만 카운트/리셋)")]
    public Transform tilesRoot;

    [Header("리셋 위치(퍼즐 실패/성공 시 되돌릴 위치)")]
    public Transform playerResetPoint;

    [Header("퍼즐 활성화")]
    public bool isActive = false;

    [Header("페이드(UI Image)")]
    public Image fadeImage;
    public float fadeOutTime = 0.25f;
    public float fadeInTime = 0.25f;
    public float fadeHoldTime = 0.05f;

    private bool isFading = false;

    private int totalTiles;
    private int steppedTiles;
    private MagicStudyTile[] tiles;

    // 자동 참조
    private Transform player;
    private PuzzleGridController gridController;
    private Rigidbody2D playerRb;

    // 퍼즐 시작/종료 시 껐다 켤 플레이어 스크립트들
    private MonoBehaviour[] disableWhenPuzzleStarts;

    void Awake()
    {
        AutoBind();
        CacheTiles();
        ResetTilesOnly();

        // ✅ 시작 시 페이드 이미지가 깜빡이지 않게 알파 0 고정
        if (fadeImage != null)
        {
            var c = fadeImage.color;
            fadeImage.color = new Color(c.r, c.g, c.b, 0f);
        }
    }

    void AutoBind()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p == null)
        {
            Debug.LogError("Player 태그 오브젝트를 찾지 못함!");
            return;
        }

        player = p.transform;
        playerRb = p.GetComponent<Rigidbody2D>();
        gridController = p.GetComponent<PuzzleGridController>();

        if (gridController == null)
            Debug.LogError("Player에 PuzzleGridController가 없음! (Player에 컴포넌트 붙여야 함)");

        disableWhenPuzzleStarts = p.GetComponents<MonoBehaviour>();
    }

    void CacheTiles()
    {
        if (tilesRoot == null)
        {
            Debug.LogError("tilesRoot 연결 안 됨!");
            tiles = new MagicStudyTile[0];
            return;
        }

        tiles = tilesRoot.GetComponentsInChildren<MagicStudyTile>(true);
        foreach (var t in tiles)
            t.manager = this;
    }

    // ===============================
    // 퍼즐 시작
    // ===============================
    public void StartPuzzle(Vector3 startCenter)
    {
        if (isActive) return;
        if (isFading) return; // ✅ 페이드 중엔 시작 금지(안전)

        isActive = true;

        CacheTiles();
        ResetTilesOnly();

        // 퍼즐 시작 시 플레이어 기존 스크립트 OFF
        if (disableWhenPuzzleStarts != null)
        {
            foreach (var mb in disableWhenPuzzleStarts)
            {
                if (mb == null) continue;
                if (mb is PuzzleGridController) continue;
                mb.enabled = false;
            }
        }

        // 플레이어를 시작 타일 중앙에 고정
        if (player != null)
        {
            var bodyCol = player.GetComponent<Collider2D>();
            Vector3 pivotOffset = bodyCol != null
                ? (bodyCol.bounds.center - player.position)
                : Vector3.zero;

            Vector3 targetPos =
                new Vector3(startCenter.x, startCenter.y, player.position.z)
                - pivotOffset;

            if (playerRb != null)
            {
                playerRb.velocity = Vector2.zero;
                playerRb.angularVelocity = 0f;
                playerRb.position = (Vector2)targetPos;
            }

            player.position = targetPos;
        }

        // 그리드 이동 ON
        if (gridController != null)
        {
            gridController.manager = this;
            gridController.enabled = true;
            gridController.gridEnabled = true;
            gridController.SetOrigin(startCenter);
        }

        Debug.Log("퍼즐 시작!");
    }

    // ===============================
    // 타일 카운트
    // ===============================
    public void RegisterStep()
    {
        steppedTiles++;
    }

    public bool IsAllTilesStepped()
    {
        return totalTiles > 0 && steppedTiles >= totalTiles;
    }

    // ===============================
    // 실패/성공 처리 (페이드 적용)
    // ===============================
    public void Fail_FallingTile()
    {
        if (isFading) return;
        Debug.Log("떨어져버렸어…");

        // ✅ 페이드 후 '텔레포트 리셋'
        StartCoroutine(FadeRoutine(() => ResetPuzzle()));
    }

    public void Fail_ReturnPlatform()
    {
        if (isFading) return;
        Debug.Log("앗 책들이 다시 올라오잖아!");

        // ✅ 페이드 후 '플레이어 위치 유지 리셋'
        StartCoroutine(FadeRoutine(() => ResetTilesAndStopPuzzle_KeepPlayer()));
    }

    public void ClearPuzzle()
    {
        if (isFading) return;
        Debug.Log("성공했다!");

        // 성공은 페이드 없이 원래대로
        ResetPuzzle();

        // 원하면 성공도 페이드 넣을 수 있음:
        // StartCoroutine(FadeRoutine(() => ResetPuzzle()));
    }

    // ===============================
    // 페이드 코루틴
    // ===============================
    IEnumerator FadeRoutine(Action midAction)
    {
        if (fadeImage == null)
        {
            // 페이드 UI 없으면 그냥 실행
            midAction?.Invoke();
            yield break;
        }

        isFading = true;

        // 페이드 중 입력/이동 막기(안전)
        if (gridController != null)
            gridController.gridEnabled = false;

        // Fade Out (0 -> 1)
        float t = 0f;
        Color baseColor = fadeImage.color;
        while (t < fadeOutTime)
        {
            t += Time.deltaTime;
            float a = (fadeOutTime <= 0f) ? 1f : Mathf.Clamp01(t / fadeOutTime);
            fadeImage.color = new Color(baseColor.r, baseColor.g, baseColor.b, a);
            yield return null;
        }
        fadeImage.color = new Color(baseColor.r, baseColor.g, baseColor.b, 1f);

        // 중간 처리(리셋)
        midAction?.Invoke();

        if (fadeHoldTime > 0f)
            yield return new WaitForSeconds(fadeHoldTime);

        // Fade In (1 -> 0)
        t = 0f;
        while (t < fadeInTime)
        {
            t += Time.deltaTime;
            float a = 1f - ((fadeInTime <= 0f) ? 1f : Mathf.Clamp01(t / fadeInTime));
            fadeImage.color = new Color(baseColor.r, baseColor.g, baseColor.b, a);
            yield return null;
        }
        fadeImage.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0f);

        isFading = false;
    }

    // ===============================
    // 기존 리셋 (플레이어 이동 포함)
    // ===============================
    public void ResetPuzzle()
    {
        ResetTilesOnly();
        isActive = false;

        // grid OFF
        if (gridController != null)
        {
            gridController.gridEnabled = false;
            gridController.enabled = false;
        }

        // 플레이어 스크립트 다시 ON
        if (disableWhenPuzzleStarts != null)
        {
            foreach (var mb in disableWhenPuzzleStarts)
            {
                if (mb == null) continue;
                if (mb is PuzzleGridController) continue;
                mb.enabled = true;
            }
        }

        // 리셋 위치로 이동
        if (player != null && playerResetPoint != null)
        {
            Vector2 p2 = playerResetPoint.position;

            if (playerRb != null)
            {
                playerRb.velocity = Vector2.zero;
                playerRb.angularVelocity = 0f;
                playerRb.position = p2;
            }

            player.position = new Vector3(p2.x, p2.y, player.position.z);
        }
    }

    // ===============================
    // 플레이어 위치 유지 + 타일만 초기화
    // ===============================
    public void ResetTilesAndStopPuzzle_KeepPlayer()
    {
        ResetTilesOnly();
        isActive = false;

        // grid OFF
        if (gridController != null)
        {
            gridController.gridEnabled = false;
            gridController.enabled = false;
        }

        // 플레이어 스크립트 다시 ON
        if (disableWhenPuzzleStarts != null)
        {
            foreach (var mb in disableWhenPuzzleStarts)
            {
                if (mb == null) continue;
                if (mb is PuzzleGridController) continue;
                mb.enabled = true;
            }
        }
    }

    // ===============================
    // 타일만 리셋
    // ===============================
    private void ResetTilesOnly()
    {
        totalTiles = 0;
        steppedTiles = 0;

        if (tiles == null) return;

        foreach (var t in tiles)
        {
            t.ResetTile();
            if (t.countForClear) totalTiles++;
        }
    }
}