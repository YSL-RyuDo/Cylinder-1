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

        if (CurrentSlot != null)
        {
            CurrentSlot.ClearSlot();
            CurrentSlot = null;
        }

        cg.blocksRaycasts = false;

        transform.SetParent(rootCanvas.transform, true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (rootCanvas == null) return;
        rect.anchoredPosition += eventData.delta / rootCanvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
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


