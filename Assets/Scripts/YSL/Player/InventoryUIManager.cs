using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class InventoryUIManager : MonoBehaviour
{
    public static InventoryUIManager Instance;

    public GameObject itemUIPrefab;
    public Transform contentParent;

    public InventorySelectionUI selectionUI;  // 선택 UI 참조
    public int CurrentItemId { get; private set; } = -1;
    private bool isPortalMode = false;
    private PortalTrigger portalForUse = null;

    // **아이템 사용 요청 관련 변수는 PlayerUIController에 있으므로 여기서는 기록하지 않음**

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SetPortalMode(bool enabled, PortalTrigger portal)
    {
        isPortalMode = enabled;
        portalForUse = portal;
    }

    public void Refresh()
    {
        // 기존 UI 클리어
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        List<int> itemIds = InventoryManager.Instance.GetAllItemIds();

        foreach (int itemId in itemIds)
        {
            int count = InventoryManager.Instance.GetItemCount(itemId);
            ItemDatas data = ItemManager.Instance.GetItemById(itemId);
            if (data == null) continue;

            GameObject go = Instantiate(itemUIPrefab, contentParent);

            // 아이콘 설정
            Image iconImage = go.transform.Find("ItemImage").GetComponent<Image>();
            Sprite iconSprite = ItemManager.Instance.GetItemIconById(data.id);
            if (iconSprite != null) iconImage.sprite = iconSprite;

            // 이름 설정
            TMP_Text nameText = go.transform.Find("ItemNameText").GetComponent<TMP_Text>();
            nameText.text = data.itemName;

            // 클릭 이벤트 추가 (Button 컴포넌트 필요)
            Button btn = go.GetComponent<Button>();
            if (btn != null)
            {
                int capturedId = itemId; // 클로저 문제 방지
                btn.onClick.AddListener(() =>
                {
                    selectionUI.ShowSelectedItem(capturedId);
                });
            }
        }

        // 선택 UI 초기화
        selectionUI.Hide();
    }

    // 아이템 사용 시도 함수
    public void TryUseSelectedItem(int itemId)
    {
        if (isPortalMode && portalForUse != null)
        {
            // PlayerUIController에 아이템 사용 요청 신호 전달
            PlayerUIController.Instance.RequestPortalUnlock(itemId);

            // 인벤토리 UI 닫기 (UI 닫힘 시 아이템 잠금 해제 시도 코루틴 안에서 처리)
            PlayerUIController.Instance.CloseMenu();

            // 포탈 모드 초기화는 UI 닫히는 코루틴에서 처리
        }
        else
        {
            Debug.Log($"아이템 {itemId} 사용 시도 (포탈 모드 아님)");
        }
    }

    public void ClearItemInfoUI()
    {
        selectionUI.Hide();
    }
}
