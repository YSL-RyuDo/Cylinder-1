using UnityEngine;
using System.IO;

public class DialogueLoader
{
    public static DialogueNodeJSON LoadDialogue(string fileName)
    {
        TextAsset textAsset = Resources.Load<TextAsset>($"Dialogues/{fileName}");
        return JsonUtility.FromJson<DialogueNodeJSON>(textAsset.text);
    }
}