using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySelectionUI : MonoBehaviour
{
    public GameObject useButton;
    public Image chooseImage;
    public TMP_Text itemNameText;
    public TMP_Text itemDescriptionText;

    private int currentSelectedItemId = -1;

    private void Awake()
    {
        useButton.SetActive(false);
        chooseImage.gameObject.SetActive(false);
        itemNameText.gameObject.SetActive(false);
        itemDescriptionText.gameObject.SetActive(false);

        useButton.GetComponent<Button>().onClick.AddListener(OnUseButtonClicked);
    }

    public void ShowSelectedItem(int itemId)
    {
        currentSelectedItemId = itemId;

        var itemData = ItemManager.Instance.GetItemById(itemId);
        if (itemData == null) return;

        useButton.SetActive(true);
        chooseImage.gameObject.SetActive(true);
        itemNameText.gameObject.SetActive(true);
        itemDescriptionText.gameObject.SetActive(true);

        chooseImage.sprite = ItemManager.Instance.GetItemIconById(itemId);
        itemNameText.text = itemData.itemName;
        itemDescriptionText.text = itemData.description;
    }

    private void OnUseButtonClicked()
    {
        if (currentSelectedItemId < 0) return;

        InventoryUIManager.Instance.TryUseSelectedItem(currentSelectedItemId);
    }

    public void Hide()
    {
        useButton.SetActive(false);
        chooseImage.gameObject.SetActive(false);
        itemNameText.gameObject.SetActive(false);
        itemDescriptionText.gameObject.SetActive(false);
        currentSelectedItemId = -1;
    }
}
