using UnityEngine;

public class Pushable : MonoBehaviour
{
    public float moveDuration = 0.2f; // 밀릴 때 걸리는 시간
    public float gridSize = 1f;       // 한 칸 크기 (기본 1)

    private bool isMoving = false;
    private Vector3 targetPos;
    private Transform pushingPlayer = null;

    public bool TryPush(Vector2 direction, Transform player = null)
    {
        if (isMoving)
        {
            Debug.Log("[Pushable] 이미 이동 중 → 실패");
            return false;
        }

        Vector3 dest = transform.position + (Vector3)(direction.normalized * gridSize);
        Debug.Log($"[Pushable] 밀기 시도: {transform.position} -> {dest}");

        Collider2D hit = Physics2D.OverlapBox(dest, Vector2.one * 0.8f, 0f, LayerMask.GetMask("Obstacle"));
        if (hit != null)
        {
            Debug.Log($"[Pushable] Obstacle({hit.name}) 때문에 실패");
            return false;
        }

        Collider2D pushPuzzleHit = Physics2D.OverlapBox(dest, Vector2.one * 0.8f, 0f, LayerMask.GetMask("Default"));
        if (pushPuzzleHit != null && pushPuzzleHit.gameObject != gameObject && pushPuzzleHit.CompareTag("PushPuzzle"))
        {
            Debug.Log($"[Pushable] 다른 PushPuzzle({pushPuzzleHit.name}) 때문에 실패");
            return false;
        }

        Debug.Log("[Pushable] 이동 시작!");
        targetPos = dest;
        isMoving = true;
        pushingPlayer = player;

        StartCoroutine(MoveToTarget(direction));
        return true;
    }

    private System.Collections.IEnumerator MoveToTarget(Vector2 direction)
    {
        Vector3 start = transform.position;
        Vector3 playerStart = pushingPlayer != null ? pushingPlayer.position : Vector3.zero;
        Vector3 playerTarget = pushingPlayer != null ? playerStart + (Vector3)(direction.normalized * gridSize) : Vector3.zero;

        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);

            transform.position = Vector3.Lerp(start, targetPos, t);

            if (pushingPlayer != null)
                pushingPlayer.position = Vector3.Lerp(playerStart, playerTarget, t);

            yield return null;
        }

        transform.position = targetPos;

        if (pushingPlayer != null)
            pushingPlayer.position = playerTarget;

        isMoving = false;
        pushingPlayer = null;
    }
}
