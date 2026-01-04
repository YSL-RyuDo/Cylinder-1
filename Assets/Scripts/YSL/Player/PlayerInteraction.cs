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
        // 대화 중이면 무조건 Next
        if (DialogueUI.Instance != null && DialogueUI.Instance.IsOpen)
        {
            if (input.InteractPressed)
            {
                DialogueManager.Instance.Next();
            }
            return;
        }

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

            NPCDialogueTrigger npc =
                results[i].GetComponentInParent<NPCDialogueTrigger>();

            if (npc != null)
            {
                // 이 NPC 전용 완료 플래그 확인
                if (!string.IsNullOrEmpty(npc.doneFlagKey) &&
                    DialogueFlagTable.Instance != null &&
                    DialogueFlagTable.Instance.Get(npc.doneFlagKey) == "true")
                {
                    // 이 NPC는 아이콘도 표시하지 않음
                    continue;
                }

                nextSprite = talkSprite;
                break;
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

        for (int i = 0; i < count; i++)
        {
            if (!results[i]) continue;

            IInteractable interactable =
                results[i].GetComponentInParent<IInteractable>();

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
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
#endif
}
