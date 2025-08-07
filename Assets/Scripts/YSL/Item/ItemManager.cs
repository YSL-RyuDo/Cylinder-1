using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;

    [Header("JSON 파일 (Resources 폴더 내)")]
    public string jsonFileName = "Items";

    private Dictionary<int, ItemDatas> itemDict = new Dictionary<int, ItemDatas>();

    public IReadOnlyDictionary<int, ItemDatas> Items => itemDict;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadItemsFromJson();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadItemsFromJson()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(jsonFileName);
        if (jsonFile == null)
        {
            Debug.LogError($"[ItemManager] JSON 파일을 찾을 수 없습니다: Resources/{jsonFileName}.json");
            return;
        }

        ItemDataList itemList = JsonUtility.FromJson<ItemDataList>(jsonFile.text);
        itemDict.Clear();
        foreach (var item in itemList.items)
        {
            itemDict[item.id] = item;
        }

        Debug.Log($"[ItemManager] {itemDict.Count}개의 아이템이 로드되었습니다.");
    }

    public ItemDatas GetItemById(int id)
    {
        if (itemDict.TryGetValue(id, out var item))
        {
            return item;
        }

        Debug.LogWarning($"[ItemManager] ID {id}에 해당하는 아이템을 찾을 수 없습니다.");
        return null;
    }

    public Sprite GetItemIconById(int id)
    {
        if (itemDict.TryGetValue(id, out var item))
        {
            return Resources.Load<Sprite>($"ItemIcons/{item.iconName}");
        }
        return null;
    }

}
