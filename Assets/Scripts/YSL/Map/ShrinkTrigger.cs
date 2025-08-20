using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ShrinkTrigger : MonoBehaviour
{
    public float maxDarkness = 0.5f;     // 최대 어둡기 (0 = 원래 색, 0.5 = 50% 어둡게)

    private BoxCollider2D triggerCollider;

    private Transform playerTransform;
    private SpriteRenderer playerRenderer;
    private Color originalColor;

    private void Awake()
    {
        triggerCollider = GetComponent<BoxCollider2D>();
        triggerCollider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerTransform = other.transform;
        playerRenderer = other.GetComponent<SpriteRenderer>();
        if (playerRenderer == null) return;

        originalColor = playerRenderer.color;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (playerTransform == null || playerRenderer == null) return;

        float distanceY = Mathf.Clamp(playerTransform.position.y - triggerCollider.bounds.min.y, 0f, triggerCollider.bounds.size.y);
        float t = distanceY / triggerCollider.bounds.size.y; // 0 = 아래, 1 = 위

        Color darkColor = Color.Lerp(originalColor, originalColor * (1f - maxDarkness), t);
        darkColor.a = originalColor.a; // 알파 유지
        playerRenderer.color = darkColor;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (playerTransform == null || playerRenderer == null) return;

        playerRenderer.color = originalColor;

        playerTransform = null;
        playerRenderer = null;
    }
}
