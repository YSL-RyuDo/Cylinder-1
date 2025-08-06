using System.Collections.Generic;
using UnityEngine;



public class DialogueDatas
{
    public List<PortalLockedDialogue> portal_locked_dialogues;
}

[System.Serializable]
public class PortalLockedDialogue
{
    public int personality_id;
    public string personality_name;
    public List<DialogueLine> lines;
}

[System.Serializable]
public class DialogueLine
{
    public List<string> segments;
}