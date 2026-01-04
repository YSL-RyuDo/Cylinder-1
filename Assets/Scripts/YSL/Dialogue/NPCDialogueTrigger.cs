using UnityEngine;

public class NPCDialogueTrigger : MonoBehaviour, IInteractable
{
    public int startSceneId;

    // 이 NPC가 확인할 플래그
    public string doneFlagKey;   // 예: "dl_done0"

    public void Interact(GameObject player)
    {
        // 대화 진행 중이면 Next
        if (DialogueUI.Instance != null
            && DialogueUI.Instance.IsOpen
            && DialogueManager.Instance != null
            && DialogueManager.Instance.CurrentNodeIsValid())
        {
            DialogueManager.Instance.Next();
            return;
        }

        // 이 NPC 전용 플래그 체크
        if (!string.IsNullOrEmpty(doneFlagKey)
            && DialogueFlagTable.Instance != null
            && DialogueFlagTable.Instance.Get(doneFlagKey) == "true")
        {
            // 이 NPC만 비활성
            return;
        }

        DialogueManager.Instance.StartDialogueByScene(startSceneId);
    }
}
