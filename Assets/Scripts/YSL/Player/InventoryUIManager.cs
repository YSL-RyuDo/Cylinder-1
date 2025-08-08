using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class InventoryUIManager : MonoBehaviour
{
    public static InventoryUIManager Instance;

    public GameObject itemUIPrefab;
    public Transform contentParent;

    public InventorySelectionUI selectionUI;
    public int CurrentItemId { get; private set; } = -1;

    private bool isPortalMode = false;
    private PortalTrigger portalForUse = null;

    private bool isPuzzleMode = false;
    private PuzzleTrigger currentPuzzleTrigger;
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

        if (enabled)
            Debug.Log("인벤토리: 포탈 모드 활성화");
        else
            Debug.Log("인벤토리: 포탈 모드 비활성화");
    }

    public void SetPuzzleMode(bool enabled, PuzzleTrigger trigger)
    {
        isPuzzleMode = enabled;
        currentPuzzleTrigger = trigger;

        if (enabled)
            Debug.Log("인벤토리: 퍼즐 모드 활성화");
        else
            Debug.Log("인벤토리: 퍼즐 모드 비활성화");
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

            // 클릭 이벤트 추가
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

    public void TryUseSelectedItem(int itemId)
    {
        Debug.Log($"TryUseSelectedItem 호출 - isPuzzleMode: {isPuzzleMode}, currentPuzzleTrigger: {currentPuzzleTrigger}");

        if (isPortalMode && portalForUse != null)
        {
            PlayerUIController.Instance.RequestPortalUnlock(itemId);
            PlayerUIController.Instance.CloseMenu();
        }
        else if (isPuzzleMode && currentPuzzleTrigger != null)
        {
            currentPuzzleTrigger.TryUseItemOnPuzzle(itemId);
            PlayerUIController.Instance.CloseMenu();
            Debug.Log($"퍼즐 모드에서 아이템 {itemId} 사용 시도");
        }
        else
        {
            Debug.Log($"아이템 {itemId} 사용 시도 (일반 모드)");
        }
    }


    public void ClearItemInfoUI()
    {
        selectionUI.Hide();
    }
}
