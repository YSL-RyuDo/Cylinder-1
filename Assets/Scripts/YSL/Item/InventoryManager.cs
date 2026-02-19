using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    private Dictionary<int, int> items = new Dictionary<int, int>();

    // 현재 사용 예정으로 등록된 아이템
    public ItemData registeredItem;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public Dictionary<int, int> GetAllItems()
    {
        return items;
    }

    public void AddItem(ItemData item)
    {
        if (!items.ContainsKey(item.id))
            items[item.id] = 0;

        items[item.id]++;
    }

    public void RemoveItem(int id)
    {
        if (!items.ContainsKey(id)) return;

        items[id]--;

        if (items[id] <= 0)
            items.Remove(id);
    }


    public void RegisterItem(ItemData item)
    {
        registeredItem = item;
        Debug.Log($"등록됨: {item.itemName}");
        UIManager.Instance.UpdateItemTabIcon(item.icon);
    }

    public ItemData GetRegisteredItem()
    {
        return registeredItem;
    }

    public void ClearRegisteredItem()
    {
        if (registeredItem != null)
        {
            RemoveItem(registeredItem.id);
        }

        registeredItem = null;

        UIManager.Instance.ResetItemTabIcon();
    }

    public void UseItem(ItemData item)
    {
        if (DialogueItemSlotUI.Instance == null)
            return;

        bool success = DialogueItemSlotUI.Instance.TryInsertItem(item);

        if (success)
        {
            RemoveItem(item.id);
        }
    }
}
