using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogueDatas
{
    public List<PortalLockedDialogue> portal_locked_dialogues;
    public List<PortalUnlockedDialogue> portal_unlocked_dialogues;

    public List<PersonalitySwitchFailDialogue> personality_switch_fail_dialogues;
    public List<PersonalitySwitchSuccessDialogue> personality_switch_success_dialogues;

    public List<SurgeryRoomEnter0Dialogues> surgery_room_enter_dialogues;
}

[Serializable]
public class PortalLockedDialogue
{
    public int personality_id;
    public string personality_name;
    public List<DialogueLine> lines;
}

[Serializable]
public class PortalUnlockedDialogue
{
    public int personality_id;
    public string personality_name;
    public List<DialogueLine> lines;
}

[Serializable]
public class PersonalitySwitchFailDialogue
{
    public int personality_id;
    public string personality_name;
    public List<DialogueLine> lines;
}

[Serializable]
public class PersonalitySwitchSuccessDialogue
{
    public int personality_id;
    public string personality_name;
    public List<DialogueLine> lines;
}

[Serializable]
public class SurgeryRoomEnter0Dialogues
{
    public int personality_id;
    public string personality_name;
    public List<DialogueLine> lines;
}

[Serializable]
public class DialogueLine
{
    public List<string> segments;
}
