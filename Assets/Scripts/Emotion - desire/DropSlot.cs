using UnityEngine;
using UnityEngine.EventSystems;

public class DropSlot : MonoBehaviour, IDropHandler
{
    [Header("이 슬롯의 인덱스 (0~4)")]
    public int slotIndex = 0;

    public DraggableSigil CurrentSigil { get; private set; }

    public void OnDrop(PointerEventData eventData)
    {
        var sigil = eventData.pointerDrag ? eventData.pointerDrag.GetComponent<DraggableSigil>() : null;
        if (sigil == null) return;

        // 이미 차 있으면 드롭 불가 → 원위치
        if (CurrentSigil != null)
        {
            sigil.ReturnToOrigin();
            return;
        }

        // ✅ 문양이 이전 슬롯에 있었으면 그 슬롯 비우기
        if (sigil.CurrentSlot != null)
        {
            sigil.CurrentSlot.ClearSlot();
        }

        // 슬롯에 고정
        CurrentSigil = sigil;
        sigil.CurrentSlot = this;

        sigil.transform.SetParent(transform, false);

        var rt = sigil.GetComponent<RectTransform>();
        rt.anchoredPosition = Vector2.zero;

        // 매니저에게 "변경됨" 알림 (카운트 체크)
        if (DesireSpellManager.Instance != null)
            DesireSpellManager.Instance.NotifySlotChanged();
    }

    public void ClearSlot()
    {
        CurrentSigil = null;
    }
}

