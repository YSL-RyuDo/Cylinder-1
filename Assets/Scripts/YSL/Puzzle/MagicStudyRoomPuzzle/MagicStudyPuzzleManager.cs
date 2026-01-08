using UnityEngine;

public class MagicStudyPuzzleManager : MonoBehaviour
{
    [Header("일반 발판 타일 부모(여기 아래만 카운트/리셋)")]
    public Transform tilesRoot;

    [Header("리셋 위치(퍼즐 실패/성공 시 되돌릴 위치)")]
    public Transform playerResetPoint;

    [Header("퍼즐 활성화")]
    public bool isActive = false;

    private int totalTiles;
    private int steppedTiles;
    private MagicStudyTile[] tiles;
    private Transform player;
    private PuzzleGridController gridController;
    private Rigidbody2D playerRb;
    private MonoBehaviour[] disableWhenPuzzleStarts;

    void Awake()
    {
        AutoBind();
        CacheTiles();
        ResetTilesOnly();
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

    public void StartPuzzle(Vector3 startCenter)
    {
        if (isActive) return;
        isActive = true;

        CacheTiles();
        ResetTilesOnly();

        if (disableWhenPuzzleStarts != null)
        {
            foreach (var mb in disableWhenPuzzleStarts)
            {
                if (mb == null) continue;
                if (mb is PuzzleGridController) continue;
                mb.enabled = false;
            }
        }

        if (player != null)
        {
            var bodyCol = player.GetComponent<Collider2D>();
            Vector3 pivotOffset = bodyCol != null ? (bodyCol.bounds.center - player.position) : Vector3.zero;

            Vector3 targetPos = new Vector3(startCenter.x, startCenter.y, player.position.z) - pivotOffset;

            if (playerRb != null)
            {
                playerRb.velocity = Vector2.zero;
                playerRb.angularVelocity = 0f;
                playerRb.position = (Vector2)targetPos;
            }

            player.position = targetPos;
        }

        if (gridController != null)
        {
            gridController.manager = this;
            gridController.enabled = true;
            gridController.gridEnabled = true;
            gridController.SetOrigin(startCenter);
        }

        Debug.Log("퍼즐 시작!");
    }

    public void RegisterStep()
    {
        steppedTiles++;
    }

    public bool IsAllTilesStepped()
    {
        return totalTiles > 0 && steppedTiles >= totalTiles;
    }

    public void Fail_FallingTile()
    {
        Debug.Log("떨어져버렸어…");
        ResetPuzzle();
    }

    public void Fail_ReturnPlatform()
    {
        Debug.Log("앗 책들이 다시 올라오잖아!");
        ResetPuzzle();
    }

    public void ClearPuzzle()
    {
        Debug.Log("성공했다!");
        ResetPuzzle();
    }

    public void ResetPuzzle()
    {
        ResetTilesOnly();

        isActive = false;

        // grid 끄기
        if (gridController != null)
        {
            gridController.gridEnabled = false;
            gridController.enabled = false;
        }

        // 플레이어 스크립트 다시 켜기
        if (disableWhenPuzzleStarts != null)
        {
            foreach (var mb in disableWhenPuzzleStarts)
            {
                if (mb == null) continue;
                if (mb is PuzzleGridController) continue;
                mb.enabled = true;
            }
        }

        // 리셋 위치 이동
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
