using UnityEngine;
using UnityEngine.UI;

public class ItemPanelUI : MonoBehaviour
{
    public GameObject itemButtonPrefab;
    public Transform contentParent;
    public Text itemDescriptionName;
    public Text itemDescriptionText;
    public Image itemDescriptionIcon;
    public Button useButton;
    public Sprite invisibleIcon;

    private PlayerScript player;

    private void OnEnable()
    {
        player = FindObjectOfType<PlayerScript>();
        RefreshItemList();
    }

    void RefreshItemList()
    {
        // 기존 버튼 제거
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        foreach (var item in player.inventory)
        {
            GameObject buttonObj = Instantiate(itemButtonPrefab, contentParent);

            // 아이콘 설정
            Transform iconTransform = buttonObj.transform.Find("Icon");
            if (iconTransform != null)
            {
                Image iconImage = iconTransform.GetComponent<Image>();
                if (iconImage != null && item.itemData.icon != null)
                {
                    iconImage.sprite = item.itemData.icon;
                    iconImage.preserveAspect = true;
                }
            }

            // 텍스트 설정
            Transform nameTextTransform = buttonObj.transform.Find("NameText");
            if (nameTextTransform != null)
            {
                Text nameText = nameTextTransform.GetComponent<Text>();
                if (nameText != null)
                    nameText.text = item.itemData.itemName;
            }

            // 클릭 이벤트 등록
            Button button = buttonObj.GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                itemDescriptionName.text = item.itemData.itemName;
                itemDescriptionText.text = item.itemData.description;
                itemDescriptionIcon.sprite = item.itemData.icon;

                useButton.onClick.RemoveAllListeners();
                useButton.onClick.AddListener(() =>
                {
                    player.RemoveItem(item.itemData);
                    itemDescriptionName.text = "";
                    itemDescriptionText.text = "";
                    itemDescriptionIcon.sprite = invisibleIcon;
                    RefreshItemList();
                });
            });
        }
    }
}
