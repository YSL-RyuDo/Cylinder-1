using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public MenuManager menuManager;
    public enum TabType { Item, Save, Map, Quest }
    public TabType tabType;

    private Image image;
    private RectTransform rectTransform;

    public Sprite defaultSprite;
    public Sprite hoverSprite;
    public Sprite selectedSprite;

    private Vector2 defaultPosition;
    public float selectedOffsetX = -10f; // ¿ÞÂÊÀ¸·Î 10px

    private void Awake()
    {
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        defaultPosition = rectTransform.anchoredPosition;
    }

    private void Start()
    {
        UpdateVisual();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsSelected())
            image.sprite = hoverSprite;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!IsSelected())
            image.sprite = defaultSprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        menuManager.OnTabSelected(this);
    }

    public void UpdateVisual()
    {
        if (IsSelected())
        {
            image.sprite = selectedSprite;
            rectTransform.anchoredPosition = defaultPosition + new Vector2(selectedOffsetX, 0);
        }
        else
        {
            image.sprite = defaultSprite;
            rectTransform.anchoredPosition = defaultPosition;
        }
    }

    private bool IsSelected()
    {
        return menuManager.currentSelectedTab == this;
    }
}
