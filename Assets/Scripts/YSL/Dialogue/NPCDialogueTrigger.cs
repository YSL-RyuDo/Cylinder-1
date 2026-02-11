using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDialogueTrigger : MonoBehaviour, IInteractable
{
    public int startSceneId;
    public string doneFlagKey;

    private float lockUntil = 0f;

    public void Interact(GameObject player)
    {
        if (Time.time < lockUntil) return;

        // 대화 진행 중이면 Next
        if (DialogueUI.Instance != null
            && DialogueUI.Instance.IsOpen
            && DialogueManager.Instance != null
            && DialogueManager.Instance.CurrentNodeIsValid())
        {
            DialogueManager.Instance.Next();
            lockUntil = Time.time + 0.2f;
            return;
        }

        // 플래그 체크 (대소문자 안전)
        if (!string.IsNullOrEmpty(doneFlagKey)
            && DialogueFlagTable.Instance != null)
        {
            var v = DialogueFlagTable.Instance.Get(doneFlagKey);
            if (string.Equals(v?.Trim(), "true", StringComparison.OrdinalIgnoreCase))
                return;
        }

        //  현재 대화 NPC 등록
        if (DialogueManager.Instance != null)
            DialogueManager.Instance.SetCurrentNpc(gameObject);

        DialogueManager.Instance.StartDialogueByScene(startSceneId);
        lockUntil = Time.time + 0.1f;
    }
}
