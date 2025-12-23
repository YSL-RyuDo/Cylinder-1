using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Collider2D))]
public class MapBoundary : MonoBehaviour
{
    public Vector2 min;
    public Vector2 max;

    private void Reset()
    {
        Recalculate();
    }

    private void OnValidate()
    {
        Recalculate();
    }

    public void Recalculate()
    {
        Collider2D col = GetComponent<Collider2D>();
        Bounds b = col.bounds;

        min = b.min;
        max = b.max;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 center = (min + max) * 0.5f;
        Vector3 size = new Vector3(max.x - min.x, max.y - min.y, 0.1f);
        Gizmos.DrawWireCube(center, size);
    }
}
