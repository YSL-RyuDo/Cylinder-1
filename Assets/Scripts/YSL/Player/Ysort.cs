using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class StaticObjectYSort : MonoBehaviour
{
    public int precision = 100;

    private SpriteRenderer sr;
    private Collider2D col;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        // 시작 시 한 번만 계산해서 고정
        sr.sortingOrder = Mathf.RoundToInt(-col.bounds.min.y * precision);
    }
}