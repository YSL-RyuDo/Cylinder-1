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

        Vector3 newPos = transform.position;
        Vector3 delta = new Vector3(input.x, input.y, 0) * moveSpeed * Time.deltaTime;

        // X축 이동
        Vector3 xMovePos = newPos + new Vector3(delta.x, 0, 0);
        if (playerCollider != null)
        {
            Collider2D[] xHits = Physics2D.OverlapBoxAll(
                xMovePos + (Vector3)playerCollider.offset,
                playerCollider.size,
                0f);

            bool blockedX = false;
            foreach (var hit in xHits)
            {
                if (hit != null && hit.gameObject != gameObject && hit.CompareTag("Obstacle"))
                {
                    blockedX = true;
                    break;
                }
            }

            if (!blockedX)
                newPos.x = xMovePos.x;
        }

        // Y축 이동
        Vector3 yMovePos = new Vector3(newPos.x, newPos.y + delta.y, 0);
        if (playerCollider != null)
        {
            Collider2D[] yHits = Physics2D.OverlapBoxAll(
                yMovePos + (Vector3)playerCollider.offset,
                playerCollider.size,
                0f);

            bool blockedY = false;
            foreach (var hit in yHits)
            {
                if (hit != null && hit.gameObject != gameObject && hit.CompareTag("Obstacle"))
                {
                    blockedY = true;
                    break;
                }
            }

            if (!blockedY)
                newPos.y = yMovePos.y;
        }

        UpdateCurrentBoundaries();

        // 박스콜라이더 크기 기준 맵 바운더리 제한
        if (CurrentMapBoundary != null)
        {
            Vector2 halfSize = playerCollider.size / 2f;

            float minX = CurrentMapBoundary.min.x + halfSize.x;
            float maxX = CurrentMapBoundary.max.x - halfSize.x;
            float minY = CurrentMapBoundary.min.y + halfSize.y;
            float maxY = CurrentMapBoundary.max.y - halfSize.y;

            newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
            newPos.y = Mathf.Clamp(newPos.y, minY, maxY);
        }

        // 원형 바운더리 제한
        if (CurrentCircularBoundary != null)
        {
            Vector2 halfSize = playerCollider.size / 2f;
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
