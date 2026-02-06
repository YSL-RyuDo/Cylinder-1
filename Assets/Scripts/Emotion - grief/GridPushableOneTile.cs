using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class GridPushableOneTile : MonoBehaviour, IGriefPushable
{
    [Header("한 칸 이동 설정")]
    [Tooltip("한 칸의 월드 좌표 크기(타일 1칸이 1이면 1, 0.5면 0.5)")]
    public float tileSize = 1f;

    [Tooltip("밀릴 때 장애물로 취급할 레이어")]
    public LayerMask obstacleMask;

    [Tooltip("이동 시간(0이면 즉시 이동)")]
    public float moveTime = 0.08f;

    private bool moving;

    public void GriefPush(Vector2 direction, float forceIgnored)
    {
        if (moving) return;

        Vector2 dir = Snap4Dir(direction);
        Vector3 start = transform.position;
        Vector3 target = start + (Vector3)(dir * tileSize);

        // 목적지에 장애물이 있으면 이동 금지
        if (IsBlocked(target))
            return;

        if (moveTime <= 0f)
        {
            transform.position = target;
            return;
        }

        StartCoroutine(MoveSmooth(start, target, moveTime));
    }

    private Vector2 Snap4Dir(Vector2 d)
    {
        // 상하좌우 중 하나로 스냅 (대각선 입력 들어와도 한 방향만)
        if (Mathf.Abs(d.x) >= Mathf.Abs(d.y))
            return d.x >= 0 ? Vector2.right : Vector2.left;
        else
            return d.y >= 0 ? Vector2.up : Vector2.down;
    }

    private bool IsBlocked(Vector3 targetPos)
    {
        // 내 콜라이더 크기 기준으로 목표 위치에서 겹치는지 검사
        var col = GetComponent<Collider2D>();
        Bounds b = col.bounds;

        // 목표 위치 기준 bounds 계산(현재 bounds를 위치만 이동)
        Vector2 size = b.size * 0.95f; // 살짝 줄여서 끼임 방지
        Vector2 center = (Vector2)targetPos + ((Vector2)b.center - (Vector2)transform.position);

        Collider2D hit = Physics2D.OverlapBox(center, size, 0f, obstacleMask);
        if (hit == null) return false;

        // 자기 자신은 무시
        if (hit.transform == transform) return false;

        return true;
    }

    private System.Collections.IEnumerator MoveSmooth(Vector3 from, Vector3 to, float t)
    {
        moving = true;
        float elapsed = 0f;

        while (elapsed < t)
        {
            elapsed += Time.deltaTime;
            float a = Mathf.Clamp01(elapsed / t);
            transform.position = Vector3.Lerp(from, to, a);
            yield return null;
        }

        transform.position = to;
        moving = false;
    }

    private void OnDrawGizmosSelected()
    {
        // 디버그용: 타일 사이즈 확인
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.05f);
    }
}