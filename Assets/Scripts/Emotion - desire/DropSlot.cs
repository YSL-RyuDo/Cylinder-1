using UnityEngine;
using UnityEngine.EventSystems;

public class DropSlot : MonoBehaviour, IDropHandler
{
    [Header("이 슬롯의 인덱스 (0~4)")]
    public int slotIndex = 0;

    public DraggableSigil CurrentSigil { get; private set; }

    public void OnDrop(PointerEventData eventData)
    {
        var sigil = eventData.pointerDrag?.GetComponent<DraggableSigil>();
        if (sigil == null) return;

        // 이미 차있으면 드롭 불가 → 원위치 복귀
        if (CurrentSigil != null)
        {
            sigil.ReturnToOrigin();
            return;
        }

        // 슬롯에 배치
        CurrentSigil = sigil;
        sigil.transform.SetParent(transform, false);

        // 슬롯 정중앙에 딱 고정
        var rt = sigil.GetComponent<RectTransform>();
        rt.anchoredPosition = Vector2.zero;

        // 매니저에게 알림
        DesireSpellManager.Instance?.NotifySlotFilled(slotIndex, sigil.sigilId);
    }

    public void ClearSlot()
    {
        CurrentSigil = null;
    }
}
