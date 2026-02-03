using UnityEngine;

public class PuzzleClickRaycaster2D : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask candleLayer;
    [SerializeField] private float candlePlaneZ = 0f;
    [SerializeField] private float hitRadius = 0.25f;

    private void Awake()
    {
        if (cam == null) cam = Camera.main;
    }

    private void Update()
    {
        var ctrl = CandlePuzzleController.Instance;
        if (ctrl == null) return;
        if (ctrl.State != CandlePuzzleController.GameState.PuzzleView) return;

        if (!Input.GetMouseButtonDown(0)) return;

        Vector3 mp = Input.mousePosition;

        float zDist = candlePlaneZ - cam.transform.position.z;
        mp.z = zDist;

        Vector3 w = cam.ScreenToWorldPoint(mp);
        Vector2 worldPos = new Vector2(w.x, w.y);

        Collider2D hitCol = Physics2D.OverlapCircle(worldPos, hitRadius, candleLayer);

        if (hitCol == null)
        {
            Debug.Log($"[Click] nothing hit at {worldPos} (planeZ={candlePlaneZ}, zDist={zDist})");
            return;
        }

        Candle candle = hitCol.GetComponentInParent<Candle>();
        if (candle == null)
        {
            Debug.Log($"[Click] hit {hitCol.name} but no Candle in parent");
            return;
        }

        Debug.Log($"[Click] hit candle: {candle.name}");
        candle.TryLight();
    }
}
