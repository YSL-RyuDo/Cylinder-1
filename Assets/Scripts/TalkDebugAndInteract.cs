using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class TalkDebugAndInteract : MonoBehaviour
{
    public KeyCode key = KeyCode.F;
    public string playerTag = "Player";

    private bool inRange;

    void Awake()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;

        var rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
    }

    void Update()
    {
        if (inRange && Input.GetKeyDown(key))
        {
            Debug.Log("[TALK] F pressed  (상호작용 입력 확인됨)");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[TALK] Enter: {other.name}, tag={other.tag}");

        if (other.CompareTag(playerTag))
        {
            inRange = true;
            Debug.Log("[TALK] Player in range  (트리거 감지됨)");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            inRange = false;
            Debug.Log("[TALK] Player out range");
        }
    }
}
