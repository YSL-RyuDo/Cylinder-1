using UnityEngine;

public class DesireSpellManager : MonoBehaviour
{
    public static DesireSpellManager Instance { get; private set; }

    [Header("슬롯 5개 연결")]
    public DropSlot[] slots = new DropSlot[5];

    private int filledCount = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void NotifySlotFilled(int slotIndex, int sigilId)
    {
        Debug.Log($"[Desire] Slot[{slotIndex}]에 문양 ID={sigilId} 들어감");

        RecountFilled();

        if (filledCount >= 5)
        {
            Debug.Log("[Desire] ✅ 5개 문양 완성! 마법 발동(디버그)");
            // 오늘은 여기까지. 나중에 실제 마법/이펙트 연결.
        }
    }

    private void RecountFilled()
    {
        int cnt = 0;
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null && slots[i].CurrentSigil != null)
                cnt++;
        }
        filledCount = cnt;
    }

    // 옵션: 리셋하고 싶을 때 호출용
    public void ResetAll()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null) continue;
            if (slots[i].CurrentSigil != null)
                slots[i].CurrentSigil.ReturnToOrigin();
            slots[i].ClearSlot();
        }
        filledCount = 0;
        Debug.Log("[Desire] reset");
    }
}

