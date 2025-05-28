using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/Node")]
public class DialogueNode : ScriptableObject
{
    [System.Serializable]
    public struct DialogueData
    {
        public string sentence;
        public Sprite leftSprite;
        public Sprite rightSprite;

        public bool hasChoices;
        public Choice[] choices;
        public string systemName;
    }


    [System.Serializable]
    public struct Choice
    {
        public string choiceText;
        public DialogueNode nextNode;
    }

    public DialogueData[] dialogues;
}