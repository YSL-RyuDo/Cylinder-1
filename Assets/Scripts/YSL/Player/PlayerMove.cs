using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;

    private PlayerInput input;
    private BoxCollider2D col;
    private Animator anim;

    public CameraFollow cameraFollow;

    public MapBoundary CurrentMapBoundary { get; private set; }
    public CircularMapBoundary CurrentCircularBoundary { get; private set; }

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        col = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
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
        Vector3 newPos = transform.position;
        Vector3 delta = (Vector3)input.MoveAxis * moveSpeed * Time.deltaTime;
        newPos += delta;

        ClampToBoundaries(ref newPos);
        transform.position = newPos;
    }

    private void UpdateAnimation()
    {
        bool walking = input.MoveAxis.sqrMagnitude > 0.01f;
        anim?.SetBool("IsWalk", walking);

        if (input.MoveAxis.x > 0.01f)
            transform.localScale = new Vector3(-1, 1, 1);
        else if (input.MoveAxis.x < -0.01f)
            transform.localScale = new Vector3(1, 1, 1);
    }

    private void ClampToBoundaries(ref Vector3 pos)
    {
        if (CurrentMapBoundary != null)
        {
            Vector2 half = col.size / 2f;
            pos.x = Mathf.Clamp(pos.x, CurrentMapBoundary.min.x + half.x, CurrentMapBoundary.max.x - half.x);
            pos.y = Mathf.Clamp(pos.y, CurrentMapBoundary.min.y + half.y, CurrentMapBoundary.max.y - half.y);
        }

        if (CurrentCircularBoundary != null)
        {
            Vector2 half = col.size / 2f;
            float limit = CurrentCircularBoundary.radius - Mathf.Max(half.x, half.y);

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

    private void UpdateCurrentBoundaries()
    {
        int mapLayerMask = LayerMask.GetMask("Map");

        Collider2D[] hits = Physics2D.OverlapPointAll(transform.position, mapLayerMask);

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

        if (cameraFollow != null)
            cameraFollow.AdjustCameraSizeToMap();
    }
}
