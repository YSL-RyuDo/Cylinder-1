using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInteraction : MonoBehaviour
{
    public float interactRange = 1.2f;
    public LayerMask interactLayer;

    private PlayerInput input;
    private bool lastShow = false;

    private readonly string[] validTags =
        { "Item", "Portal", "Door", "Puzzle" };  // UI를 띄워줄 대상

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
        Collider2D target = Physics2D.OverlapCircle(transform.position, interactRange, interactLayer);

        bool shouldShow = false;

        if (target != null)
        {
            foreach (var tag in validTags)
            {
                if (target.CompareTag(tag))
                {
                    shouldShow = true;
                    break;
                }
            }
        }

        // 변화가 있을 때만 UI 갱신
        if (shouldShow != lastShow)
        {
            if (shouldShow)
                ObjectInteractionUI.Instance.Show();
            else
                ObjectInteractionUI.Instance.Hide();

            lastShow = shouldShow;
        }
    }

    private void TryInteract()
    {
        Collider2D target = Physics2D.OverlapCircle(transform.position, interactRange, interactLayer);
        if (target == null) return;

        IInteractable interactable = target.GetComponent<IInteractable>();
        if (interactable != null)
            interactable.Interact(this.gameObject);
    }
}
