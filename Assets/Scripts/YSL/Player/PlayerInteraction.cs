using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInteraction : MonoBehaviour
{
    public float interactRange = 1.2f;

    public LayerMask interactableLayer;
    public LayerMask npcLayer;
    public LayerMask portalLayer;

    public SpriteRenderer interactionRenderer;
    public Sprite normalSprite;
    public Sprite talkSprite;
    public Sprite lockedSprite;

    private PlayerInput input;
    private readonly Collider2D[] results = new Collider2D[8];
    private Sprite lastSprite = null;

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        interactionRenderer.sprite = null;
    }

    private void Update()
    {
        bool canInteract = UpdateInteractionSprite();

        if (canInteract && input.InteractPressed)
        {
            TryInteract();
        }
    }

    private bool UpdateInteractionSprite()
    {
        int count = Physics2D.OverlapCircleNonAlloc(
            transform.position,
            interactRange,
            results,
            interactableLayer | npcLayer | portalLayer
        );

        Sprite nextSprite = null;

        for (int i = 0; i < count; i++)
        {
            if (!results[i]) continue;

            if (((1 << results[i].gameObject.layer) & npcLayer) != 0)
            {
                nextSprite = talkSprite;
                break;
            }
        }

        if (nextSprite == null)
        {
            for (int i = 0; i < count; i++)
            {
                if (!results[i]) continue;

                if (((1 << results[i].gameObject.layer) & interactableLayer) != 0)
                {
                    nextSprite = normalSprite;
                    break;
                }
            }
        }

        if (nextSprite == null)
        {
            for (int i = 0; i < count; i++)
            {
                if (!results[i]) continue;

                if (((1 << results[i].gameObject.layer) & portalLayer) != 0)
                {
                    PortalTrigger p = results[i].GetComponent<PortalTrigger>();
                    if (p != null && p.data.isLocked)
                    {
                        nextSprite = lockedSprite;
                    }
                    break;
                }
            }
        }

        if (lastSprite != nextSprite)
        {
            interactionRenderer.sprite = nextSprite;
            lastSprite = nextSprite;
        }

        return nextSprite != null;
    }

    private void TryInteract()
    {
        int count = Physics2D.OverlapCircleNonAlloc(
            transform.position,
            interactRange,
            results,
            interactableLayer | npcLayer | portalLayer
        );

        Debug.Log($"[Interact] Hit Count = {count}");

        for (int i = 0; i < count; i++)
        {
            if (!results[i]) continue;

            GameObject hitObj = results[i].gameObject;
            int layer = hitObj.layer;
            string layerName = LayerMask.LayerToName(layer);

            Debug.Log(
                $"[Interact] Hit Object = {hitObj.name}, " +
                $"Layer = {layerName} ({layer})"
            );

            IInteractable interactable =
                results[i].GetComponentInParent<IInteractable>();

            if (interactable != null)
            {
                Debug.Log($"[Interact] IInteractable FOUND on {hitObj.name}");
                interactable.Interact(gameObject);
                return;
            }
            else
            {
                Debug.Log($"[Interact] IInteractable NOT found on {hitObj.name}");
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
#endif
}
