using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class YSort : MonoBehaviour
{
    [Header("정렬 기준")]
    [Tooltip("정렬 기준이 되는 위치 Transform. 비워두면 자기 자신")]
    public Transform pivot;

    [Header("정렬 보정")]
    [Tooltip("같은 Y값에서의 우선순위 보정값")]
    public int orderOffset = 0;

    [Header("정밀도")]
    [Tooltip("Y값을 정수로 변환할 때 사용하는 배율")]
    public int precision = 100;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (pivot == null)
            pivot = transform;
    }

    private void LateUpdate()
    {
        ApplySorting();
    }

    private void ApplySorting()
    {
        int sortingOrder = Mathf.RoundToInt(-pivot.position.y * precision);
        spriteRenderer.sortingOrder = sortingOrder + orderOffset;
    }
}
