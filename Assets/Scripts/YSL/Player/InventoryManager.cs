//using System;
//using System.Collections.Generic;
//using UnityEngine;

//[Serializable]
//public class InventoryItem
//{
//    public int itemId;
//    public int count;
//}

//public class InventoryManager : MonoBehaviour
//{
//    public static InventoryManager Instance;

//    [SerializeField]
//    private List<InventoryItem> inventoryList = new List<InventoryItem>();

//    private Dictionary<int, int> inventory = new Dictionary<int, int>();

//    private void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//            // 리스트 -> 딕셔너리 초기 변환
//            inventory.Clear();
//            foreach (var item in inventoryList)
//            {
//                if (inventory.ContainsKey(item.itemId))
//                    inventory[item.itemId] += item.count;
//                else
//                    inventory[item.itemId] = item.count;
//            }
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }

//    private void OnValidate()
//    {
//        // 딕셔너리 -> 리스트 동기화 (인스펙터 표시용)
//        inventoryList.Clear();
//        foreach (var kvp in inventory)
//        {
//            inventoryList.Add(new InventoryItem() { itemId = kvp.Key, count = kvp.Value });
//        }
//    }

//    /// <summary>
//    /// 아이템 추가. 성공하면 true 반환
//    /// </summary>
//    public bool AddItem(int itemId, int count = 1)
//    {
//        if (itemId < 0)
//        {
//            Debug.LogWarning("[InventoryManager] 잘못된 아이템 ID: " + itemId);
//            return false;
//        }

//        if (inventory.ContainsKey(itemId))
//        {
//            inventory[itemId] += count;
//        }
//        else
//        {
//            inventory[itemId] = count;
//        }

//        Debug.Log($"[InventoryManager] 아이템 {itemId} 추가됨. 현재 수량: {inventory[itemId]}");

//        OnValidate(); // 리스트 동기화

//        return true;
//    }

//    /// <summary>
//    /// 아이템 수량 조회
//    /// </summary>
//    public int GetItemCount(int itemId)
//    {
//        if (inventory.TryGetValue(itemId, out int count))
//        {
//            return count;
//        }
//        return 0;
//    }

//    /// <summary>
//    /// 아이템 삭제 (count 만큼 제거)
//    /// </summary>
//    public bool RemoveItem(int itemId, int count = 1)
//    {
//        if (!inventory.ContainsKey(itemId))
//            return false;

//        int currentCount = inventory[itemId];
//        if (currentCount < count)
//        {
//            Debug.LogWarning($"[InventoryManager] 아이템 {itemId} 제거 실패: 요청 수량({count})이 현재 수량({currentCount})보다 많음.");
//            return false;
//        }

//        inventory[itemId] -= count;

//        if (inventory[itemId] <= 0)
//        {
//            inventory.Remove(itemId);

//            // 선택된 아이템 UI가 해당 아이템이면 초기화
//            InventoryUIManager uiManager = FindObjectOfType<InventoryUIManager>();
//            if (uiManager != null && uiManager.CurrentItemId == itemId)
//            {
//                uiManager.ClearItemInfoUI();
//            }
//        }

//        Debug.Log($"[InventoryManager] 아이템 {itemId} 제거됨. 현재 수량: {GetItemCount(itemId)}");

//        OnValidate(); // 리스트 동기화

//        return true;
//    }



//    /// <summary>
//    /// 현재 인벤토리에 있는 모든 아이템 ID 반환
//    /// </summary>
//    public List<int> GetAllItemIds()
//    {
//        return new List<int>(inventory.Keys);
//    }
//    public bool HasItem(int itemId)
//    {
//        return GetItemCount(itemId) > 0;
//    }
//}
