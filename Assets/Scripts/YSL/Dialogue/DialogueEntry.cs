using System;

// 인격 전환
[Serializable]
public class PersonalityDialogueCollection
{
    public PersonalityDialogue[] personalities;
}

[Serializable]
public class PersonalityDialogue
{
    public int id;
    public string folder;
    public string name;
    public DialogueBlock preview;
    public DialogueBlock confirm;
}

[Serializable]
public class DialogueBlock
{
    public string emotion;   // 스프라이트 이름
    public string[] lines;   // 대사들
}

// 포탈 열기
[System.Serializable]
public class PortalMonologueCollection
{
    public PortalMonologue[] portalMonologues;
}

[System.Serializable]
public class PortalMonologue
{
    public int id;
    public string folder;
    public string name;

    public DialogueBlock missingItem; // 아이템 없음
    public DialogueBlock wrongItem;   // 틀린 아이템
}
