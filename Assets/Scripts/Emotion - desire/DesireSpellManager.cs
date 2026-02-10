using UnityEngine;

public class DesireSpellManager : MonoBehaviour
{
    public static DesireSpellManager Instance { get; private set; }

    [Header("슬롯 5개 연결 (Inspector에서 0~4에 각각 드래그)")]
    public DropSlot[] slots = new DropSlot[5];

    // ✅ 한 번 발동하면 계속 유지
    public bool SpellActive { get; private set; } = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // DropSlot에서 드롭 성공 시 호출
    public void NotifySlotChanged()
    {
        // 이미 발동했으면 유지이므로 더 체크할 필요는 없음(원하면 계속 로그는 남길 수 있음)
        if (SpellActive) return;

        int filled = GetFilledCount();
        Debug.Log($"[Desire] filled = {filled}/5");

        if (filled >= 5)
        {
            SpellActive = true;
            Debug.Log("[Desire] ✅ 5개 완성! 열망 마법 발동(이후 계속 유지)");
        }
    }

    public int GetFilledCount()
    {
        int cnt = 0;
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null && slots[i].CurrentSigil != null)
                cnt++;
        }
        return cnt;
    }
}


