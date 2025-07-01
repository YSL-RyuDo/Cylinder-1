using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventManage : MonoBehaviour
{
    public GameObject FadeIn;
    public GameObject WhiteBackground;
    public DialogueNode startDialogueNode;
    bool gameStartEvent = false;
    void Start()
    {
        TalkManagerScript.instance.Ondialogue(startDialogueNode.dialogues);
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameStartEvent && !FadeIn.activeSelf && !WhiteBackground.activeSelf)
        {
            PlayerScript player = FindObjectOfType<PlayerScript>();
            player.AutoMoveTo(new Vector2(31.0f, -5.5f),1f);
            gameStartEvent = true;
        }
    }
}
