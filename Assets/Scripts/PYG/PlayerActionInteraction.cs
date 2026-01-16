using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerActionInteraction : MonoBehaviour
{
    public float interactRange = 1.2f;
    public LayerMask objectLayer;

    public SpriteRenderer interactionRenderer;
    public Sprite actionSprite;

    private PlayerInput input;
    private readonly Collider2D[] results = new Collider2D[8];
    private Sprite lastSprite = null;
    private bool lastInteractPressed = false;

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        if (DialogueUI.Instance != null && DialogueUI.Instance.IsOpen)
        {
            if (interactionRenderer != null) interactionRenderer.sprite = null;
            return;
        }

        bool pressed = input.InteractPressed;

        bool canInteract = UpdateActionIcon();

        if (canInteract && pressed && !lastInteractPressed)
        {
            TryAction();
        }

        lastInteractPressed = pressed;
    }

    private bool UpdateActionIcon()
    {
        int count = Physics2D.OverlapCircleNonAlloc(
            transform.position,
            interactRange,
            results,
            objectLayer
        );

        Sprite nextSprite = null;
        bool foundTarget = false;

        for (int i = 0; i < count; i++)
        {
            if (!results[i]) continue;

            IInteractable interactable = results[i].GetComponentInParent<IInteractable>();

            if (interactable != null)
            {
                nextSprite = actionSprite;
                foundTarget = true;
                break;
            }
        }

        if (interactionRenderer != null && lastSprite != nextSprite)
        {
            interactionRenderer.sprite = nextSprite;
            lastSprite = nextSprite;
        }

        return foundTarget;
    }

    private void TryAction()
    {
        int count = Physics2D.OverlapCircleNonAlloc(
            transform.position,
            interactRange,
            results,
            objectLayer
        );

        for (int i = 0; i < count; i++)
        {
            if (!results[i]) continue;

            IInteractable interactable = results[i].GetComponentInParent<IInteractable>();

            if (interactable != null)
            {
                interactable.Interact(gameObject);
                return;
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow; // 기존과 구분되게 노란색
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
#endif
}