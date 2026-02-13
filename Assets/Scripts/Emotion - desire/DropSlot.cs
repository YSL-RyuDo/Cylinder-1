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

        if (CurrentSigil != null)
        {
            sigil.ReturnToOrigin();
            return;
        }

        if (sigil.CurrentSlot != null)
        {
            sigil.CurrentSlot.ClearSlot();
        }

        CurrentSigil = sigil;
        sigil.CurrentSlot = this;

        sigil.transform.SetParent(transform, false);

        var rt = sigil.GetComponent<RectTransform>();
        rt.anchoredPosition = Vector2.zero;

        if (DesireSpellManager.Instance != null)
            DesireSpellManager.Instance.NotifySlotChanged();
    }

    public void ClearSlot()
    {
        CurrentSigil = null;
    }
}

