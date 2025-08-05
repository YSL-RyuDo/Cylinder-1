using UnityEngine;

[ExecuteAlways]
public class CircularMapBoundary : MonoBehaviour
{
    public Vector2 min;
    public Vector2 max;
    public Vector2 center;

    public float radius;

    void Start()
    {
        CalculateBounds();
    }

    public void CalculateBounds()
    {
        SpriteRenderer[] srs = GetComponentsInChildren<SpriteRenderer>();
        if (srs.Length == 0)
        {
            Debug.LogWarning("하위에 SpriteRenderer 없음");
            min = max = center = Vector2.zero;
            radius = 0f;
            return;
        }

        Bounds combinedBounds = srs[0].bounds;
        for (int i = 1; i < srs.Length; i++)
        {
            combinedBounds.Encapsulate(srs[i].bounds);
        }

        min = combinedBounds.min;
        max = combinedBounds.max;

        Vector2 internalCenter = (min + max) * 0.5f;

        // center가 (0,0)이라면 내부 계산값 사용, 아니면 외부 할당값 유지
        if (center == Vector2.zero)
        {
            center = internalCenter;
        }

        float width = max.x - min.x;
        float height = max.y - min.y;

        radius = Mathf.Max(width, height) * 0.5f;
    }

    private void OnDrawGizmos()
    {
        if (radius <= 0f) return;

        Gizmos.color = Color.cyan;
        DrawCircle(new Vector3(center.x, center.y, transform.position.z), radius, 60);
    }

    void DrawCircle(Vector3 center, float radius, int segments)
    {
        float step = 360f / segments;
        Vector3 prev = center + new Vector3(Mathf.Cos(0), Mathf.Sin(0), 0) * radius;

        for (int i = 1; i <= segments; i++)
        {
            float rad = Mathf.Deg2Rad * step * i;
            Vector3 next = center + new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * radius;
            Gizmos.DrawLine(prev, next);
            prev = next;
        }
    }
}
