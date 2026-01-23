using System.Collections;
using UnityEngine;

public class PuzzleGridController : MonoBehaviour
{
    [Header("이동")]
    public float moveSpeed = 6f;

    [Header("그리드 셀 크기 (수동 설정)")]
    public float cellSizeX = 1.1f;
    public float cellSizeY = 1.1f;

    public bool gridEnabled = false;

    [Header("퍼즐 매니저(자동 연결됨)")]
    public MagicStudyPuzzleManager manager;

    [Header("레이어 마스크")]
    public LayerMask obstacleMask;
    public LayerMask groundMask;

    [Header("장애물 체크(이동 전)")]
    [Range(0.1f, 0.95f)]
    public float obstacleCheckRatio = 0.55f;

    [Header("바닥 체크(이동 후 실패조건1)")]
    public Vector2 groundCheckOffset = new Vector2(0f, -0.3f);
    public float groundCheckRadius = 0.05f;

    [Header("그리드 기준점(스타트 타일 중앙)")]
    public Vector3 gridOrigin;
    public bool hasOrigin = false;

    [Header("플레이어 기준 콜라이더(비워두면 자동 탐색)")]
    public Collider2D bodyCollider;

    // 콜라이더 중심 - transform.position
    private Vector3 pivotOffset;
    private bool moving = false;

    void Awake()
    {
        if (bodyCollider == null)
            bodyCollider = GetComponent<Collider2D>();

        RecalcPivotOffset();
    }

    void RecalcPivotOffset()
    {
        if (bodyCollider != null)
            pivotOffset = bodyCollider.bounds.center - transform.position;
        else
            pivotOffset = Vector3.zero;
    }

    public void SetOrigin(Vector3 originWorldPos)
    {
        RecalcPivotOffset();

        gridOrigin = new Vector3(
            originWorldPos.x,
            originWorldPos.y,
            transform.position.z
        );

        hasOrigin = true;

        transform.position = SnapToGrid(transform.position);
    }

    private Vector3 SnapToGrid(Vector3 transformPos)
    {
        if (!hasOrigin) return transformPos;

        // 기준점: 콜라이더 중심
        Vector3 refPos = transformPos + pivotOffset;

        float x = Mathf.Round((refPos.x - gridOrigin.x) / cellSizeX) * cellSizeX + gridOrigin.x;
        float y = Mathf.Round((refPos.y - gridOrigin.y) / cellSizeY) * cellSizeY + gridOrigin.y;

        Vector3 snappedRef = new Vector3(x, y, refPos.z);

        // 다시 transform 위치로 환산
        return snappedRef - pivotOffset;
    }

    void Update()
    {
        if (!gridEnabled) return;
        if (manager == null || !manager.isActive) return;
        if (moving) return;

        Vector3 dir = Vector3.zero;
        if (Input.GetKeyDown(KeyCode.W)) dir = Vector3.up;
        else if (Input.GetKeyDown(KeyCode.S)) dir = Vector3.down;
        else if (Input.GetKeyDown(KeyCode.A)) dir = Vector3.left;
        else if (Input.GetKeyDown(KeyCode.D)) dir = Vector3.right;
        if (dir == Vector3.zero) return;

        // 이동 시작 전 스냅
        transform.position = SnapToGrid(transform.position);

        Vector3 step = Vector3.zero;
        if (dir == Vector3.left) step = new Vector3(-cellSizeX, 0f, 0f);
        else if (dir == Vector3.right) step = new Vector3(cellSizeX, 0f, 0f);
        else if (dir == Vector3.up) step = new Vector3(0f, cellSizeY, 0f);
        else if (dir == Vector3.down) step = new Vector3(0f, -cellSizeY, 0f);

        Vector3 target = SnapToGrid(transform.position + step);

        // 장애물 체크 (X/Y 분리)
        float sizeX = Mathf.Max(0.2f, cellSizeX * obstacleCheckRatio);
        float sizeY = Mathf.Max(0.2f, cellSizeY * obstacleCheckRatio);
        Vector2 checkSize = new Vector2(sizeX, sizeY);

        Vector2 targetCenter = (Vector2)(target + pivotOffset);
        if (Physics2D.OverlapBox(targetCenter, checkSize, 0f, obstacleMask) != null)
            return;

        StartCoroutine(MoveTo(target));
    }

    IEnumerator MoveTo(Vector3 target)
    {
        moving = true;

        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }

        transform.position = SnapToGrid(target);

        Vector2 checkPos = (Vector2)transform.position + groundCheckOffset;
        if (Physics2D.OverlapCircle(checkPos, groundCheckRadius, groundMask) == null)
        {
            manager.Fail_FallingTile();
            moving = false;
            yield break;
        }

        moving = false;
    }
}
