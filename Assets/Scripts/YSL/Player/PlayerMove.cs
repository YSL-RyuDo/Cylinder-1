using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Vector2 input;
    private BoxCollider2D playerCollider;
    private Animator animator;

    public CameraFollow cameraFollow;

    public MapBoundary CurrentMapBoundary { get; private set; }
    public CircularMapBoundary CurrentCircularBoundary { get; private set; }

    private void Awake()
    {
        playerCollider = GetComponent<BoxCollider2D>();
        if (playerCollider == null)
            Debug.LogError("플레이어에 BoxCollider2D가 없습니다!");

        animator = GetComponent<Animator>();
        if (animator == null)
            Debug.LogWarning("플레이어에 Animator가 없습니다.");
    }

    void Update()
    {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (input.sqrMagnitude > 1) input = input.normalized;

        Vector3 moveDelta = new Vector3(input.x, input.y, 0) * moveSpeed * Time.deltaTime;
        Vector3 newPos = transform.position + moveDelta;

        // 충돌 체크
        if (playerCollider != null)
        {
            // BoxCollider 크기 및 위치 기반 이동 위치에서 겹치는 콜라이더 검사
            Collider2D[] hits = Physics2D.OverlapBoxAll(
                newPos + (Vector3)playerCollider.offset,  // 새 위치 + 콜라이더 오프셋
                playerCollider.size,
                0f);

            bool blocked = false;
            foreach (var hit in hits)
            {
                if (hit != null && hit.gameObject != gameObject && hit.CompareTag("Obstacle"))
                {
                    blocked = true;
                    break;
                }
            }

            if (blocked)
            {
                // 장애물이 있어서 이동하지 않음
                newPos = transform.position;
            }
        }

        UpdateCurrentBoundaries();

        Vector2 halfSize = playerCollider.bounds.extents;
        Vector2 offset = playerCollider.offset;

        if (CurrentMapBoundary != null)
        {
            float playerBottomY = newPos.y + (offset.y - halfSize.y);
            float playerTopY = newPos.y + (offset.y + halfSize.y);

            float mapMinY = CurrentMapBoundary.min.y;
            float mapMaxY = CurrentMapBoundary.max.y;

            float minX = CurrentMapBoundary.min.x + halfSize.x;
            float maxX = CurrentMapBoundary.max.x - halfSize.x;
            newPos.x = Mathf.Clamp(newPos.x, minX, maxX);

            if (playerBottomY < mapMinY)
            {
                newPos.y = mapMinY - (offset.y - halfSize.y);
            }
            else if (playerTopY > mapMaxY)
            {
                newPos.y = mapMaxY - (offset.y + halfSize.y);
            }
        }

        if (CurrentCircularBoundary != null)
        {
            Vector2 center = CurrentCircularBoundary.center;
            float radius = CurrentCircularBoundary.radius;

            Vector2 playerPos2D = new Vector2(newPos.x, newPos.y);

            float dist = Vector2.Distance(playerPos2D, center);
            float allowedRadius = radius - Mathf.Max(halfSize.x, halfSize.y);

            if (dist > allowedRadius)
            {
                Vector2 dir = (playerPos2D - center).normalized;
                Vector2 correctedPos = center + dir * allowedRadius;
                newPos.x = correctedPos.x;
                newPos.y = correctedPos.y;
            }
        }

        transform.position = newPos;

        bool isWalking = input.sqrMagnitude > 0.01f;
        animator?.SetBool("IsWalk", isWalking);

        if (input.x > 0.01f)
            transform.localScale = new Vector3(-1, 1, 1);
        else if (input.x < -0.01f)
            transform.localScale = new Vector3(1, 1, 1);
    }

    void UpdateCurrentBoundaries()
    {
        int mapLayerMask = LayerMask.GetMask("Map");

        Collider2D[] colliders = Physics2D.OverlapPointAll(transform.position, mapLayerMask);

        MapBoundary foundMapBoundary = null;
        CircularMapBoundary foundCircularBoundary = null;

        foreach (var col in colliders)
        {
            if (foundMapBoundary == null)
                foundMapBoundary = col.GetComponent<MapBoundary>();
            if (foundCircularBoundary == null)
                foundCircularBoundary = col.GetComponent<CircularMapBoundary>();

            if (foundMapBoundary != null && foundCircularBoundary != null)
                break;
        }

        bool changed = false;

        if (foundMapBoundary != CurrentMapBoundary)
        {
            CurrentMapBoundary = foundMapBoundary;
            changed = true;
        }

        if (foundCircularBoundary != CurrentCircularBoundary)
        {
            CurrentCircularBoundary = foundCircularBoundary;
            changed = true;
        }

        if (changed && cameraFollow != null)
        {
            cameraFollow.AdjustCameraSizeToMap();
        }
    }

    public void Warp(float x, float y)
    {
        transform.position = new Vector2(x, y);
        UpdateCurrentBoundaries();

        if (cameraFollow != null)
        {
            cameraFollow.AdjustCameraSizeToMap();
        }
    }

    public void SetCurrentMapBoundary(MapBoundary boundary)
    {
        CurrentMapBoundary = boundary;
    }

    public void SetCurrentCircularBoundary(CircularMapBoundary boundary)
    {
        CurrentCircularBoundary = boundary;
    }
}
