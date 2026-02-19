using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class DialogueItemSlotUI : MonoBehaviour
{
    public static DialogueItemSlotUI Instance;

    public GameObject rootPanel;
    public GameObject slotPrefab;   // ½½·Ô ÇÁ¸®ÆÕ
    public Transform slotParent;    // ½½·ÔµéÀÌ »ý¼ºµÉ ºÎ¸ð

    private List<Image> slots = new();
    private List<ItemData> insertedItems = new();
    private List<int> correctItemIds = new();

    private void Awake()
    {
        Instance = this;
        rootPanel.SetActive(false);
    }

    public void Open(int slotCount, List<int> correctIds)
    {
        rootPanel.SetActive(true);
        correctItemIds = correctIds;

        ClearSlots();

        for (int i = 0; i < slotCount; i++)
        {
            GameObject obj = Instantiate(slotPrefab, slotParent);
            Image img = obj.GetComponent<Image>();

            img.sprite = null;
            img.color = new Color(1,1,1,0.3f);

            slots.Add(img);
            insertedItems.Add(null);
        }
    }

    public void Close()
    {
        rootPanel.SetActive(false);
        ClearSlots();
    }

    private void ClearSlots()
    {
        foreach (Transform child in slotParent)
            Destroy(child.gameObject);

        slots.Clear();
        insertedItems.Clear();
    }

    public bool TryInsertItem(ItemData item)
    {
        for (int i = 0; i < insertedItems.Count; i++)
        {
            if (insertedItems[i] == null)
            {
                insertedItems[i] = item;
                slots[i].sprite = item.icon;
                slots[i].color = Color.white;

                CheckCombination();
                return true;
            }
        }

        return false;
    }

    private void CheckCombination()
    {
        var currentIds = insertedItems
            .Where(x => x != null)
            .Select(x => x.id)
            .ToList();

        if (currentIds.Count != correctItemIds.Count)
            return;

        bool success = !correctItemIds.Except(currentIds).Any();

        if (success)
        {
            Debug.Log("ÆÛÁñ ¼º°ø");
            DialogueManager.Instance.RequestNext();
            Close();
        }
        else
        {
            Debug.Log("ÆÛÁñ ½ÇÆÐ");
        }
    }
}
