using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentencesScript : MonoBehaviour
{
    [Tooltip("Resources/Dialogues 폴더 안에 있는 json 파일 이름(확장자 제외)")]
    public string dialogueFileName;

    public void interact()
    {
        if (TalkManagerScript.instance.dialogueGroup.alpha == 0)
        {
            TalkManagerScript.instance.StartDialogue(dialogueFileName);
        }
    }
}