using UnityEngine;

public class Item : MonoBehaviour
{
    public int itemId; // JSON 아이템 ID와 매칭

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();

        
    }

    private void Start()
    {
        UpdateSprite();
        AdjustSpriteScaleToCollider();
    }

    public void Pickup()
    {
        bool success = InventoryManager.Instance.AddItem(itemId);
        if (success)
        {
            gameObject.SetActive(false);
        }
    }

    public void UpdateSprite()
    {
        if (spriteRenderer == null) return;

        if (ItemManager.Instance == null)
        {
            Debug.LogWarning("ItemManager.Instance가 아직 초기화되지 않았습니다.");
            return;
        }

        Sprite icon = ItemManager.Instance.GetItemIconById(itemId);
        if (icon != null)
        {
            spriteRenderer.sprite = icon;
        }
        else
        {
            Debug.LogWarning($"아이콘 스프라이트를 찾을 수 없습니다. 아이템ID: {itemId}");
        }
    }

    public void AdjustSpriteScaleToCollider()
    {
        if (spriteRenderer == null || boxCollider == null) return;
        if (spriteRenderer.sprite == null) return;

        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;    // 스프라이트 원본 크기 (월드 단위)
        Vector2 colliderSize = boxCollider.size;                    // 박스콜라이더 크기 (월드 단위)

        // 원본 스프라이트 크기에 대한 현재 transform.localScale을 고려한 실제 크기
        Vector2 currentSpriteWorldSize = new Vector2(
            spriteSize.x * transform.localScale.x,
            spriteSize.y * transform.localScale.y);

        // 콜라이더 크기에 맞추기 위한 스케일 비율 계산
        float scaleX = colliderSize.x / spriteSize.x;
        float scaleY = colliderSize.y / spriteSize.y;

        // Z축 스케일은 유지
        transform.localScale = new Vector3(scaleX, scaleY, transform.localScale.z);
    }


}
