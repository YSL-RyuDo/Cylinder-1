using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class DraggableSigil : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("ID (문양 구분용)")]
    public int sigilId = 0;

    private RectTransform rect;
    private Canvas rootCanvas;
    private CanvasGroup canvasGroup;

    private Transform originalParent;
    private Vector2 originalAnchoredPos;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        rootCanvas = GetComponentInParent<Canvas>();
        if (rootCanvas == null)
            Debug.LogError("[DraggableSigil] Canvas를 찾을 수 없음. 문양이 Canvas 아래에 있어야 함!");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originalAnchoredPos = rect.anchoredPosition;

        // 드래그 중엔 슬롯 레이캐스트에 걸리지 않게
        canvasGroup.blocksRaycasts = false;

        // UI가 다른 UI 위로 올라오게 (최상위로)
        transform.SetParent(rootCanvas.transform, true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (rootCanvas == null) return;

        // 마우스 위치로 이동 (Canvas Scale 고려)
        rect.anchoredPosition += eventData.delta / rootCanvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 기본은 원래 위치로 복귀 (슬롯에 못 들어갔을 때)
        if (transform.parent == rootCanvas.transform)
        {
            ReturnToOrigin();
        }

        canvasGroup.blocksRaycasts = true;
    }

    public void ReturnToOrigin()
    {
        transform.SetParent(originalParent, true);
        rect.anchoredPosition = originalAnchoredPos;
    }
}

