using UnityEngine;
using UnityEngine.UI;

public class ItemPanelUI : MonoBehaviour
{
    public GameObject itemButtonPrefab;
    public Transform contentParent;
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

        // 아이템 버튼 생성
        foreach (var item in player.inventory)
        {
            GameObject buttonObj = Instantiate(itemButtonPrefab, contentParent);

            // 버튼의 이미지 설정 (아이콘)
            Image buttonImage = buttonObj.GetComponent<Image>();
            if (buttonImage != null && item.itemData.icon != null)
            {
                buttonImage.sprite = item.itemData.icon;
                buttonImage.preserveAspect = true;
            }
            
            Text nameText = buttonObj.GetComponentInChildren<Text>();
            if (nameText != null)
                nameText.text = item.itemData.itemName;

            // 클릭 이벤트 등록
            Button button = buttonObj.GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                itemDescriptionText.text = item.itemData.description;
                itemDescriptionIcon.sprite = item.itemData.icon;

                useButton.onClick.RemoveAllListeners();
                useButton.onClick.AddListener(() =>
                {
                    player.RemoveItem(item.itemData);
                    itemDescriptionText.text = "";
                    itemDescriptionIcon.sprite = invisibleIcon;
                    RefreshItemList();
                });
            });
        }
    }
}
