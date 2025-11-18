using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance;

    public ItemData[] allItems;

    private void Awake()
    {
        Instance = this;
    }

    public ItemData GetItemById(int id)
    {
        foreach (var item in allItems)
        {
            if (item.id == id)
                return item;
        }
        return null;
    }
}
