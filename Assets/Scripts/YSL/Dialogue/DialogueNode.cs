using System;

[Serializable]
public class DialogueNode
{
    public int id;
    public int scene_id;
    public string speaker;
    public int emotion;
    public string text;

    public string condition_key;
    public string condition_value;

    public string choice0_text;
    public int choice0_next;

    public string command;
    public string command_target;
    public string command_value;

    public int next_id;
}
