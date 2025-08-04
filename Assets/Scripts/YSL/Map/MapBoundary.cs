using UnityEngine;

[ExecuteAlways]
public class MapBoundary : MonoBehaviour
{
    public Vector2 min;
    public Vector2 max;

    private void Reset()
    {
        AutoCalculateBounds();
    }

    private void OnValidate()
    {
        AutoCalculateBounds();
    }

    public void AutoCalculateBounds()
    {
        var sr = GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogWarning($"{gameObject.name}에 SpriteRenderer가 없습니다.");
            return;
        }

        Bounds bounds = sr.bounds;

        min = bounds.min;
        max = bounds.max;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 size = new Vector3(max.x - min.x, max.y - min.y, 0.1f);
        Vector3 center = new Vector3((min.x + max.x) / 2f, (min.y + max.y) / 2f, transform.position.z);
        Gizmos.DrawWireCube(center, size);
    }
}
