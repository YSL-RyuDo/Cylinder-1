using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentencesScript : MonoBehaviour
{
    public string[] sentences;
    public Sprite[] Lsprites;
    public Sprite[] Rsprites;

    public void interact()
    {
        if(TalkManagerScript.instance.dialogueGroup.alpha == 0) 
        {
            TalkManagerScript.instance.Ondialogue(sentences, Lsprites, Rsprites);
        }
    }
}
