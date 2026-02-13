using UnityEngine;

public class DesireSpellManager : MonoBehaviour
{
    public static DesireSpellManager Instance { get; private set; }

    [Header("슬롯 5개 연결")]
    public DropSlot[] slots = new DropSlot[5];

    private int filledCount = 0;

    public bool IsSpellActive { get; private set; } = false;
    private bool spellActivated = false;
    public bool IsSpellActivated => spellActivated;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void NotifySlotFilled(int slotIndex, int sigilId)
    {
        RecountFilled();

        Debug.Log($"[Desire] filled = {filledCount}/5");

        if (filledCount >= 5)
        {
            IsSpellActive = true;
            Debug.Log("[Desire] 열망 마법 발동");
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

    public void NotifySlotChanged()
    {
        RecountFilled();

        Debug.Log($"[Desire] filled = {filledCount}/5");

        if (filledCount >= 5 && !spellActivated)
        {
            spellActivated = true;
            Debug.Log("[Desire] 불 마법 발동");
        }
    }

}