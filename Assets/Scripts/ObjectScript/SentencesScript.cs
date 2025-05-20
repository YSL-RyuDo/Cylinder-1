using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentencesScript : MonoBehaviour
{
    public DialogueNode dialogueNode;

    public void interact()
    {
        if(TalkManagerScript.instance.dialogueGroup.alpha == 0) 
        {
            TalkManagerScript.instance.Ondialogue(dialogueNode.dialogues);
        }
    }
}
