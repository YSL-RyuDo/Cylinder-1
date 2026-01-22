using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class YSortByCollider : MonoBehaviour
{
    public int orderOffset = 0;
    public int precision = 100;

    private SpriteRenderer spriteRenderer;
    private Collider2D col;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        if (col == null)
        {
            Debug.LogError($"{name} : Collider2D가 필요합니다.");
        }
    }

    private void LateUpdate()
    {
        ApplySorting();
    }

    private void ApplySorting()
    {
        if (col == null) return;

        // 콜라이더 하단 Y값
        float bottomY = col.bounds.min.y;

        int sortingOrder = Mathf.RoundToInt(-bottomY * precision);
        spriteRenderer.sortingOrder = sortingOrder + orderOffset;
    }
}
