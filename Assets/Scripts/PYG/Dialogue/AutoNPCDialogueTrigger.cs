using System;
using UnityEngine;

public class AutoNPCDialogueTrigger : MonoBehaviour
{
    [Header("해당 코드 사용시, rigidbody2D의 kinematic 옵션, sleep mode 를 never sleep으로, collider2D의 isTrigger 옵션을 체크해야 합니다.")]
    public int startSceneId;
    public string doneFlagKey;
    public string requiredFlagKey;
    
    [Tooltip("체크시 선행 조건이 '달성되지 않았을 때' 실행. (False일 때 실행)")]
    public bool reverseRequiredCondition = false;

    [Tooltip("체크시 이벤트가 실행되는 즉시 requiredFlagKey를 'true'로 변경.")]
    public bool autoCompleteOnTrigger = false;

    private float lockUntil = 0f;
    private void OnTriggerStay2D(Collider2D collision)
    {
        //플레이어가 아닌 경우 무시
        if (!collision.CompareTag("Player"))
        {
            return;
        }

        //사전 대화 조건 체크
        if (!string.IsNullOrEmpty(requiredFlagKey))
        {
            if (DialogueFlagTable.Instance == null) return;

            string reqValue = DialogueFlagTable.Instance.Get(requiredFlagKey);
            bool isFlagTrue = string.Equals(reqValue?.Trim(), "true", StringComparison.OrdinalIgnoreCase);

            if (reverseRequiredCondition)
            {
                if (isFlagTrue) return;
            }
            else
            {
                if (!isFlagTrue) return;
            }
        }

        if (Time.time < lockUntil) return;

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

        if (autoCompleteOnTrigger && !string.IsNullOrEmpty(requiredFlagKey) && DialogueFlagTable.Instance != null)
        {
            DialogueFlagTable.Instance.Set(requiredFlagKey, "true");
            Debug.Log($"[AutoTrigger] '{requiredFlagKey}' 플래그가 자동으로 True로 설정되었습니다.");
        }
        DialogueManager.Instance.StartDialogueByScene(startSceneId);
        lockUntil = Time.time + 0.1f;
    }
}
