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

    public void RegisterItem(ItemData item)
    {
        registeredItem = item;
        Debug.Log($"등록됨: {item.itemName}");
        UIManager.Instance.UpdateItemTabIcon(item.icon);
    }

    public bool HasRegisteredItem()
    {
        return registeredItem != null;
    }
}
