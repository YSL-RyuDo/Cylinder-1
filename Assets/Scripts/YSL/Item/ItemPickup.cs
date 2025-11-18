using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable
{
    public ItemData item;
    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (item != null && item.icon != null)
        {
            ApplyItemSprite(item.icon);
        }
    }

    private void ApplyItemSprite(Sprite newSprite)
    {
        if (sr == null) return;

        Vector2 oldSize = sr.bounds.size;

        sr.sprite = newSprite;

        Vector2 newSize = sr.bounds.size;

        if (newSize.x == 0 || newSize.y == 0)
            return;

        Vector3 scale = transform.localScale;
        scale.x *= oldSize.x / newSize.x;
        scale.y *= oldSize.y / newSize.y;

        transform.localScale = scale;
    }

    public void Interact(GameObject player)
    {
        InventoryManager.Instance.AddItem(item);
        NarrationUI.Instance.ShowMessage($"{item.itemName}¿ª(∏¶) »πµÊ«ﬂΩ¿¥œ¥Ÿ.");

        Destroy(gameObject);
    }
}
