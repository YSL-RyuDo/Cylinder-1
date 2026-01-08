using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class MagicStudyTile : MonoBehaviour
{
    public MagicStudyPuzzleManager manager;
    public bool countForClear = true;

    private bool stepped = false;
    private bool fallen = false;

    private SpriteRenderer sr;
    private BoxCollider2D col;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();
        col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (manager == null || !manager.isActive) return;
        if (fallen) return;

        if (!stepped)
        {
            stepped = true;
            if (countForClear) manager.RegisterStep();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (manager == null || !manager.isActive) return;
        if (fallen) return;

        if (stepped) FallNow();
    }

    void FallNow()
    {
        fallen = true;
        col.enabled = false;
        if (sr) sr.enabled = false;
    }

    public void ResetTile()
    {
        stepped = false;
        fallen = false;

        if (col) { col.enabled = true; col.isTrigger = true; }
        if (sr) sr.enabled = true;
    }
}
