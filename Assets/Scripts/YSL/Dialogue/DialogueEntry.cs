using System;

[Serializable]
public class DialogueLines
{
    public string emotion;
    public string[] lines;
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
public class PersonalityDialogueCollection
{
    public PersonalityDialogue[] personalities;
}

[Serializable]
public class DialogueBlock
{
    public string emotion;   // 스프라이트 이름
    public string[] lines;   // 대사들
}