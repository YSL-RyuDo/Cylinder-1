using UnityEngine;

public class NPCDialogueTrigger : MonoBehaviour, IInteractable
{
    public int startNodeId;

    public void Interact(GameObject player)
    {
        if (DialogueUI.Instance != null && DialogueUI.Instance.IsOpen)
        {
            Debug.Log("[NPCDialogueTrigger] Dialogue Next");
            DialogueManager.Instance.Next();
        }
        else
        {
            Debug.Log($"[NPCDialogueTrigger] StartDialogue({startNodeId})");
            DialogueManager.Instance.StartDialogue(startNodeId);
        }
    }
}
