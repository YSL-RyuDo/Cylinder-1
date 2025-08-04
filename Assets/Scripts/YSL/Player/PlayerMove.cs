using UnityEngine;
using System.Collections;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Vector2 input;
    private BoxCollider2D playerCollider;
    private Animator animator;
    public CameraFollow cameraFollow;
    public MapBoundary CurrentMapBoundary { get; private set; }
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
        // 입력 받기
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (input.sqrMagnitude > 1) input = input.normalized;

        Vector3 moveDelta = new Vector3(input.x, input.y, 0) * moveSpeed * Time.deltaTime;
        Vector3 newPos = transform.position + moveDelta;

        UpdateCurrentMapBoundary();

        if (CurrentMapBoundary != null && playerCollider != null)
        {
            Vector2 halfSize = playerCollider.bounds.extents;
            Vector2 offset = playerCollider.offset;

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

        // 위치 이동
        transform.position = newPos;

        // 애니메이션 처리
        bool isWalking = input.sqrMagnitude > 0.01f;
        animator?.SetBool("IsWalk", isWalking);

        // 캐릭터 방향 전환
        if (input.x > 0.01f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (input.x < -0.01f)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    void UpdateCurrentMapBoundary()
    {
        int mapLayerMask = LayerMask.GetMask("Map");
        Collider2D mapCollider = Physics2D.OverlapPoint(transform.position, mapLayerMask);

        MapBoundary newBoundary = mapCollider != null ? mapCollider.GetComponent<MapBoundary>() : null;

        if (newBoundary != CurrentMapBoundary)
        {
            CurrentMapBoundary = newBoundary;

            if (cameraFollow != null)
            {
                cameraFollow.AdjustCameraSizeToMap();
            }
        }
    }


    public void Warp(float x, float y)
    {
        transform.position = new Vector2(x, y);

        UpdateCurrentMapBoundary();

        if (cameraFollow != null)
        {
            cameraFollow.AdjustCameraSizeToMap();
        }
    }

    public void SetCurrentMapBoundary(MapBoundary boundary)
    {
        CurrentMapBoundary = boundary;
    }


}
