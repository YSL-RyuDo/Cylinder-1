using UnityEngine;

public class PuzzleClickRaycaster2D : MonoBehaviour
{
    [SerializeField] private LayerMask candleLayer;

    private void Update()
    {
        var ctrl = CandlePuzzleController.Instance;
        if (ctrl == null) return;
        if (ctrl.State != CandlePuzzleController.GameState.PuzzleView) return;

        if (DesireSpellManager.Instance != null && !DesireSpellManager.Instance.IsSpellActivated) return;

        if (!Input.GetMouseButtonDown(0)) return;

        Camera cam = ctrl.GetMainCamera();
        if (cam == null) return;

        Vector3 w = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 worldPos = new Vector2(w.x, w.y);

        Collider2D[] hits = Physics2D.OverlapPointAll(worldPos, candleLayer);

        if (hits == null || hits.Length == 0)
        {
            Debug.Log("[Click] nothing hit");
            return;
        }

        float minDist = float.MaxValue;
        Candle nearest = null;

        foreach (var col in hits)
        {
            Candle c = col.GetComponentInParent<Candle>();
            if (c == null) continue;

            float d = Vector2.Distance(worldPos, col.bounds.ClosestPoint(worldPos));
            if (d < minDist)
            {
                minDist = d;
                nearest = c;
            }
        }

        if (nearest == null)
        {
            Debug.Log("[Click] no candle found");
            return;
        }

        Debug.Log($"[Click] hit candle: {nearest.name}");
        nearest.Light();
    }
}
