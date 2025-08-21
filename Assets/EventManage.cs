using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventManage : MonoBehaviour
{
    public GameObject FadeIn;
    public GameObject WhiteBackground;
    public string startDialogueNode;
    public bool alreadyStart = false;
    void Start()
    {
        FadeIn.SetActive(true);
        if (TalkManagerScript.instance.dialogueGroup.alpha == 0 && !alreadyStart)
        {
            TalkManagerScript.instance.StartDialogue(startDialogueNode);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}