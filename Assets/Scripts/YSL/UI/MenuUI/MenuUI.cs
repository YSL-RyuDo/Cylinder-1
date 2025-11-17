using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour, IUIWindow
{
    public bool IsOpen { get; private set; }

    [Header("Root 패널")]
    [SerializeField] private GameObject root;

    [Header("탭 버튼들 (순서 중요)")]
    public Button[] tabButtons;

    [Header("탭 패널들 (순서 중요)")]
    public GameObject[] tabPanels;

    [Header("아이템 버튼 스프라이트 변경용")]
    public int itemTabIndex = 0;
    public Sprite itemUsedSprite;

    private Sprite[] defaultSprites;
    private bool[] customMarking;

    private void Awake()
    {
        int count = tabButtons.Length;
        defaultSprites = new Sprite[count];
        customMarking = new bool[count];

        for (int i = 0; i < count; i++)
        {
            defaultSprites[i] = tabButtons[i].image.sprite;
            customMarking[i] = false;

            int index = i;
            tabButtons[i].onClick.AddListener(() => SelectTab(index));
        }
    }

    public void Open()
    {
        IsOpen = true;
        root.SetActive(true);
    }

    public void Close()
    {
        IsOpen = false;
        root.SetActive(false);

        FindAnyObjectByType<UIManager>().NotifyWindowClosed(this);
    }

    public void SelectTab(int index)
    {
        for (int i = 0; i < tabButtons.Length; i++)
        {
            bool isSelected = (i == index);

            tabPanels[i].SetActive(isSelected);

            tabButtons[i].interactable = !isSelected;

            if (!isSelected && !customMarking[i])
            {
                tabButtons[i].image.sprite = defaultSprites[i];
            }
        }
    }

    // 1) 아이템 사용 시 아이템 탭 버튼 스프라이트 교체
    public void MarkItemUsed()
    {
        customMarking[itemTabIndex] = true;
        tabButtons[itemTabIndex].image.sprite = itemUsedSprite;
    }

    public void ResetItemMark()
    {
        customMarking[itemTabIndex] = false;

        // 선택된 탭이 아닐 때만 즉시 복원 가능
        if (tabButtons[itemTabIndex].interactable == true)
        {
            tabButtons[itemTabIndex].image.sprite = defaultSprites[itemTabIndex];
        }

    }
}
