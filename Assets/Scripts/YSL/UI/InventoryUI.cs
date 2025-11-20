using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

    public GameObject root;

    [Header("Slot")]
    public GameObject slotPrefab;
    public Transform content;

    [Header("Selected Item Info")]
    public Image chooseImage;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescriptionText;
    public Button useButton;
    public bool IsOpen { get; private set; }
    private ItemData selectedItem;

    private void Awake()
    {
        Instance = this;
    }

    public void Open()
    {
        root.SetActive(true);
        Refresh();
    }

    public void Close()
    {
        root.SetActive(false);
    }

    public void Refresh()
    {
        foreach (Transform child in content)
            Destroy(child.gameObject);

        foreach (var kv in InventoryManager.Instance.GetAllItems())
        {
            ItemData item = ItemDatabase.Instance.GetItemById(kv.Key);
            GameObject go = Instantiate(slotPrefab, content);

            var slot = go.GetComponent<ItemSlot>();
            slot.Set(item, OnSelectItem);
        }
    }

    private void OnSelectItem(ItemData item)
    {
        selectedItem = item;
        chooseImage.sprite = item.icon;
        itemNameText.text = item.itemName;
        itemDescriptionText.text = item.description;

        chooseImage.gameObject.SetActive(true);
        itemNameText.gameObject.SetActive(true);
        itemDescriptionText.gameObject.SetActive(true);
        useButton.gameObject.SetActive(true);
    }

    public void OnClickUse()
    {
        if (selectedItem == null) return;

        // 아이템 등록
        InventoryManager.Instance.RegisterItem(selectedItem);
        chooseImage.gameObject.SetActive(false);
        itemNameText.gameObject.SetActive(false);
        itemDescriptionText.gameObject.SetActive(false);
        useButton.gameObject.SetActive(false);

        // 나레이션 출력 (선택사항)
        //NarrationUI.Instance.ShowMessage($"{selectedItem.itemName}을(를) 사용할 준비가 되었습니다.");

        MenuUI menu = FindAnyObjectByType<MenuUI>();
        if (menu != null && menu.IsOpen)
            menu.Close();
    }

}
