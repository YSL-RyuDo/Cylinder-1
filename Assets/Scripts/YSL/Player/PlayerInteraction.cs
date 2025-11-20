using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInteraction : MonoBehaviour
{
    public float interactRange = 1.2f;
    //public LayerMask interactLayer;

    private PlayerInput input;
    private bool lastShow = false;

    private readonly string[] validTags =
    { "Item", "Puzzle" };  // UI 띄울 '일반' 상호작용 대상

    private readonly string[] lockableTags =
    { "Portal" };   // 나중에 "Door", "Chest" 등 추가 가능

    private bool lastState = false;

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        UpdateInteractionUI();

        if (input.InteractPressed)
            TryInteract();
    }

    private void UpdateInteractionUI()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRange);

        foreach (var target in hits)
        {
            if (target == null) continue;

            // 1. 아이템 / 퍼즐
            foreach (var tag in validTags)
            {
                if (target.CompareTag(tag))
                {
                    ObjectInteractionUI.Instance.ShowNormal();
                    lastState = true;
                    return;
                }
            }

            // 2. 포탈
            foreach (var tag in lockableTags)
            {
                if (target.CompareTag(tag))
                {
                    PortalTrigger p = target.GetComponent<PortalTrigger>();

                    if (p != null && p.data.isLocked)
                    {
                        ObjectInteractionUI.Instance.ShowLocked();
                        lastState = true;
                        return;
                    }

                    ObjectInteractionUI.Instance.Hide();
                    lastState = false;
                    return;
                }
            }
        }

        // 범위 내 아무 것도 없음 → 숨김
        if (lastState)
        {
            ObjectInteractionUI.Instance.Hide();
            lastState = false;
        }
    }


    private void TryInteract()
    {
        // 범위 내 모든 Collider 검색
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRange);

        foreach (var target in hits)
        {
            if (target == null) continue;

            // 1. 일반 상호작용 (Item / Puzzle)
            foreach (var tag in validTags)
            {
                if (target.CompareTag(tag))
                {
                    IInteractable interactable = target.GetComponent<IInteractable>();
                    if (interactable != null)
                        interactable.Interact(this.gameObject);
                    return;
                }
            }

            // 2. 포탈
            foreach (var tag in lockableTags)
            {
                if (target.CompareTag(tag))
                {
                    IInteractable interactable = target.GetComponent<IInteractable>();
                    if (interactable != null)
                        interactable.Interact(this.gameObject);
                    return;
                }
            }
        }
    }

}
