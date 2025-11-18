using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlot : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI nameText;
    public Button button;

    private ItemData item;

    // InventoryUI에서 슬롯 생성 시 데이터 넣기
    public void Set(ItemData newItem, System.Action<ItemData> onClick)
    {
        item = newItem;

        icon.sprite = item.icon;
        nameText.text = item.itemName;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick(item));
    }
}
