using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(BoxCollider2D))]
public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;
    private const float SKIN = 0.01f;

    private PlayerInput input;
    private BoxCollider2D col;
    private Animator anim;
    private SpriteRenderer sr;

    public CameraFollow cameraFollow;

    [Header("Collision Masks")]
    [SerializeField] private LayerMask obstacleLayer; // 예: NPC, Obstacle
    [SerializeField] private LayerMask waterLayer;    // 예: Water

    [Header("Debug")]
    [SerializeField] private bool debugHitLog = false;

    public MapBoundary CurrentMapBoundary { get; private set; }
    public CircularMapBoundary CurrentCircularBoundary { get; private set; }

    private Collider2D[] overlapBuffer = new Collider2D[16];

    [Header("Runtime Override")]
    public bool useSpeedOverride = false;
    public float speedOverride = 5f;
    public float CurrentMoveSpeed => useSpeedOverride ? speedOverride : moveSpeed;

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        col = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        Move();
        UpdateAnimation();
    }

    private void LateUpdate()
    {
        UpdateCurrentBoundaries();
    }

    private void Move()
    {
        // 대쉬 중이면 PlayerMove는 덮어쓰기 금지
        var grief = GetComponent<GriefAbilityController>();
        if (grief != null && grief.IsDashingNow)
            return;

        Vector2 inputDir = input.MoveAxis;
        if (inputDir.sqrMagnitude < 0.0001f)
            return;

        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + (Vector3)(inputDir.normalized * CurrentMoveSpeed * Time.deltaTime);

        Vector3 finalPos = startPos;

        // X축
        Vector3 testX = new Vector3(targetPos.x, startPos.y, startPos.z);
        if (!IsBlocked(testX))
            finalPos.x = testX.x;

        // Y축
        Vector3 testY = new Vector3(finalPos.x, targetPos.y, startPos.z);
        if (!IsBlocked(testY))
            finalPos.y = testY.y;

        ClampToBoundaries(ref finalPos);
        transform.position = finalPos;
    }

    //  핵심: 물 레이어는 grief만 통과, 그 외 인격은 막기
    private bool IsBlocked(Vector3 pos)
    {
        Vector2 half = GetWorldHalfSize();
        Vector2 offset = GetWorldOffset();

        Vector2 center = (Vector2)pos + offset;
        Vector2 size = half * 2f - Vector2.one * SKIN;

        //  Water + Obstacle만 검사하도록 필터 적용
        LayerMask blockMask = obstacleLayer | waterLayer;

        int count = Physics2D.OverlapBoxNonAlloc(center, size, 0f, overlapBuffer, blockMask);

        for (int i = 0; i < count; i++)
        {
            Collider2D hit = overlapBuffer[i];
            if (hit == null) continue;
            if (hit == col) continue;

            int hitMask = 1 << hit.gameObject.layer;

            // --- Water 처리 ---
            if ((hitMask & waterLayer) != 0)
            {
                var pp = PlayerPersonality.Instance;
                bool isGrief = (pp != null && pp.currentPersonality == PersonalityType.grief);

                if (debugHitLog)
                {
                    Debug.Log($"[BLOCK CHECK] WATER hit={hit.name} isTrigger={hit.isTrigger} personality={(pp ? pp.currentPersonality.ToString() : "null")} -> {(isGrief ? "PASS" : "BLOCK")}");
                }

                // grief면 통과, 아니면 막기
                if (!isGrief)
                    return true;

                continue;
            }

            // --- Obstacle 처리 ---
            if ((hitMask & obstacleLayer) != 0)
            {
                if (debugHitLog)
                    Debug.Log($"[BLOCK CHECK] OBSTACLE hit={hit.name} layer={LayerMask.LayerToName(hit.gameObject.layer)} isTrigger={hit.isTrigger} -> BLOCK");

                return true;
            }
        }

        return false;
    }

    private void UpdateAnimation()
    {
        bool walking = input.MoveAxis.sqrMagnitude > 0.01f;
        anim?.SetBool("IsWalk", walking);

        if (input.MoveAxis.x > 0.01f)
            sr.flipX = true;
        else if (input.MoveAxis.x < -0.01f)
            sr.flipX = false;
    }

    private void ClampToBoundaries(ref Vector3 pos)
    {
        Vector2 half = GetWorldHalfSize();
        Vector2 offset = GetWorldOffset();

        if (CurrentMapBoundary != null)
        {
            pos.x = Mathf.Clamp(
                pos.x,
                CurrentMapBoundary.min.x + half.x - offset.x,
                CurrentMapBoundary.max.x - half.x - offset.x
            );

            pos.y = Mathf.Clamp(
                pos.y,
                CurrentMapBoundary.min.y + half.y - offset.y,
                CurrentMapBoundary.max.y - half.y - offset.y
            );
        }

        if (CurrentCircularBoundary != null)
        {
            float radius = Mathf.Max(half.x, half.y);
            float limit = CurrentCircularBoundary.radius - radius;

            Vector2 center = CurrentCircularBoundary.center;
            Vector2 p = pos;

            float dist = Vector2.Distance(p, center);
            if (dist > limit)
            {
                Vector2 dir = (p - center).normalized;
                pos = center + dir * limit;
            }
        }
    }

    private Vector2 GetWorldHalfSize()
    {
        Vector3 scale = transform.lossyScale;
        return new Vector2(
            col.size.x * Mathf.Abs(scale.x) * 0.5f,
            col.size.y * Mathf.Abs(scale.y) * 0.5f
        );
    }

    private Vector2 GetWorldOffset()
    {
        Vector3 scale = transform.lossyScale;
        return new Vector2(
            col.offset.x * Mathf.Abs(scale.x),
            col.offset.y * Mathf.Abs(scale.y)
        );
    }

    private void UpdateCurrentBoundaries()
    {
        int mapLayerMask = LayerMask.GetMask("Map");
        Vector2 checkPoint = (Vector2)transform.position + GetWorldOffset();

        Collider2D[] hits = Physics2D.OverlapPointAll(checkPoint, mapLayerMask);

        MapBoundary foundMapBoundary = null;
        CircularMapBoundary foundCircularBoundary = null;

        foreach (var hit in hits)
        {
            if (foundMapBoundary == null)
                foundMapBoundary = hit.GetComponent<MapBoundary>();

            if (foundCircularBoundary == null)
                foundCircularBoundary = hit.GetComponent<CircularMapBoundary>();

            if (foundMapBoundary != null && foundCircularBoundary != null)
                break;
        }

        CurrentMapBoundary = foundMapBoundary;
        CurrentCircularBoundary = foundCircularBoundary;

        cameraFollow?.AdjustCameraSizeToMap();
    }

    public void Warp(Vector2 pos)
    {
        transform.position = pos;
        UpdateCurrentBoundaries();
        cameraFollow?.AdjustCameraSizeToMap();
    }
}