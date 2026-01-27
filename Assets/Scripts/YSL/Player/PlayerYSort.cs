using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class PlayerYSort : MonoBehaviour
{
    public int precision = 100;

    private SpriteRenderer sr;
    private Collider2D col;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    void LateUpdate()
    {
        sr.sortingOrder = Mathf.RoundToInt(-col.bounds.min.y * precision);
    }
}