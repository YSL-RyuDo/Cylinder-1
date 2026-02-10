using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class DraggableSigil : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int sigilId = 0;

    [HideInInspector] public DropSlot CurrentSlot;

    private RectTransform rect;
    private Canvas rootCanvas;
    private CanvasGroup cg;

    private Transform originParent;
    private Vector2 originPos;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        cg = GetComponent<CanvasGroup>();
        rootCanvas = GetComponentInParent<Canvas>();

        if (rootCanvas == null)
            Debug.LogError("[DraggableSigil] Canvas 아래에 있어야 합니다!");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originParent = transform.parent;
        originPos = rect.anchoredPosition;

        // ✅ 슬롯에 박혀있던 상태면 슬롯을 먼저 비워줌 (그래야 다시 뺄 수 있음)
        if (CurrentSlot != null)
        {
            CurrentSlot.ClearSlot();
            CurrentSlot = null;
        }

        cg.blocksRaycasts = false;

        // 최상단으로 올려서 드래그
        transform.SetParent(rootCanvas.transform, true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (rootCanvas == null) return;
        rect.anchoredPosition += eventData.delta / rootCanvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 드롭 실패하면 아직 Canvas 루트에 남아있음 → 원위치 복귀
        if (transform.parent == rootCanvas.transform)
        {
            ReturnToOrigin();
        }

        cg.blocksRaycasts = true;
    }

    public void ReturnToOrigin()
    {
        transform.SetParent(originParent, true);
        rect.anchoredPosition = originPos;
    }
}


