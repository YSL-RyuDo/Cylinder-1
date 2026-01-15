using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButlerInteract : MonoBehaviour
{
    [Header("JSON 대화 파일 경로 (Resources/Dialogues 기준, 확장자 빼기)")]
    // 예: Assets/Resources/Dialogues/Chapter00/startevent.json
    // -> 여기엔 "Chapter00/startevent"
    [SerializeField] private string butlerinteract = "butlerinteract";

    public void interact()
    {
        if (TalkManagerScript.instance == null)
        {
            Debug.LogError("TalkManagerScript.instance가 null 입니다. 씬에 TalkManager가 있는지 확인하세요.");
            return;
        }

        // 이미 대화중이면 무시
        if (TalkManagerScript.instance.dialogueGroup != null &&
            TalkManagerScript.instance.dialogueGroup.alpha > 0)
            return;

        Debug.Log($"[ButlerInteract] StartDialogue: {butlerinteract}");
        TalkManagerScript.instance.StartDialogue(butlerinteract);
    }
}
