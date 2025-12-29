using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using System.IO;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    private Dictionary<int, DialogueNode> nodeTable = new();
    private Dictionary<int, List<DialogueSceneSlot>> sceneSlotTable = new();
    private DialogueVariableTable variables = new();

    private int currentNodeId;
    private int currentSceneId = -1;
    private bool waitingForChoice = false;

    private void Awake()
    {
        Instance = this;

        PortraitDatabase.LoadAll();
        LoadDialogueNodes("Dialogues/dialogue_Scene");
        LoadSceneSlots("Dialogues/dialogue");
    }

    public void StartDialogue(int startId)
    {
        currentNodeId = startId;
        waitingForChoice = false;

        if (!DialogueUI.Instance.IsOpen)
            UIManager.Instance.Open(DialogueUI.Instance);

        ExecuteNode(currentNodeId);
    }

    public void Next()
    {
        if (waitingForChoice)
            return;

        if (currentNodeId < 0)
        {
            DialogueUI.Instance.Close();
            return;
        }

        ExecuteNode(currentNodeId);
    }


    private void ExecuteNode(int id)
    {
        if (id < 0)
        {
            DialogueUI.Instance.Close();
            return;
        }

        if (!nodeTable.TryGetValue(id, out var node))
        {
            Debug.LogError($"[ExecuteNode] node NOT FOUND id={id}");
            return;
        }

        currentNodeId = id;

        if (node.scene_id != currentSceneId)
        {
            ApplyScene(node.scene_id);
            currentSceneId = node.scene_id;
        }

        if (!CheckCondition(node))
        {
            ExecuteNode(node.next_id);
            return;
        }

        // System 노드는 출력 없이 즉시 실행
        if (node.speaker == "System")
        {
            RunSystem(node);
            ExecuteNode(node.next_id);
            return;
        }

        // Choice는 여기서 멈춤
        if (node.speaker == "Choice")
        {
            waitingForChoice = true;
            ShowChoiceBlock(id);
            return;
        }

        // 여기서 먼저 출력
        DialogueUI.Instance.ShowLine(
            node.speaker,
            ParseText(node.text)
        );

        // 출력 후 다음 처리
        if (node.next_id < 0)
        {
            currentNodeId = -1;
            return;
        }

        if (nodeTable.TryGetValue(node.next_id, out var nextNode) &&
        (nextNode.speaker == "Choice" || nextNode.speaker == "System"))
        {
            ExecuteNode(node.next_id);
            return;
        }

        currentNodeId = node.next_id;
    }

    private void ShowChoiceBlock(int startId)
    {
        List<(string, Action)> choices = new();
        int id = startId;

        while (nodeTable.TryGetValue(id, out var n) && n.speaker == "Choice")
        {
            int next = n.choice0_next;
            string cmd = n.command;
            string tgt = n.command_target;
            string val = n.command_value;

            choices.Add((n.text, () =>
            {
                waitingForChoice = false;

                if (cmd == "set_var")
                    variables.Set(tgt, val);

                ExecuteNode(next);
            }
            ));

            id++;
        }

        currentNodeId = id;
        DialogueUI.Instance.ShowChoices(choices);
    }

    private void RunSystem(DialogueNode node)
    {
        string cmd = node.command?.Trim();
        string target = node.command_target?.Trim();
        string value = node.command_value?.Trim();

        Debug.Log($"[RunSystem] cmd='{cmd}', target='{target}', value='{value}'");

        if (cmd == "enter")
        {
            Sprite sp = null;

            if (target == "라즐리")
            {
                sp = PortraitDatabase.GetPlayer(0, 0);
            }
            else
            {
                sp = PortraitDatabase.GetNPC(target);
            }

            if (sp == null)
            {
                Debug.LogError($"[RunSystem] ENTER Sprite NOT FOUND target='{target}'");
                return;
            }

            DialogueUI.Instance.EnterSlot(value, sp, target);
            return;
        }

        if (cmd == "move")
        {
            string fromSlot = null;
            Sprite foundSprite = null;

            foreach (string slot in DialogueUI.Instance.GetAllSlots())
            {
                string speaker = DialogueUI.Instance.GetSpeakerInSlot(slot);

                if (string.IsNullOrEmpty(speaker))
                    continue;

                if (speaker.Trim().Equals(target, StringComparison.Ordinal))
                {
                    foundSprite = DialogueUI.Instance.GetSlotSprite(slot);
                    fromSlot = slot;
                    break;
                }
            }

            if (foundSprite == null)
            {
                Debug.LogError(
                    $"[RunSystem] MOVE source NOT FOUND target='{target}'"
                );
                return;
            }

            // 기존 슬롯은 비활성화 (제거가 아니라 이동)
            DialogueUI.Instance.EnterSlot(value, foundSprite, target);

            return;
        }

        Debug.LogWarning($"[RunSystem] UNKNOWN command='{cmd}'");
    }

    private void ApplyScene(int sceneId)
    {
        DialogueUI.Instance.ClearSlots();
        DialogueUI.Instance.ClearSlotSpeakers();
        if (!sceneSlotTable.TryGetValue(sceneId, out var slots))
            return;

        foreach (var s in slots)
        {
            Sprite sp = null;

            if (s.speaker == "라즐리")
            {
                sp = PortraitDatabase.GetPlayer(s.personality, s.emotion);
            }
            else
            {
                sp = PortraitDatabase.GetNPC(s.speaker);
            }

            if (sp != null)
            {
                // 화자 이름까지 함께 등록
                DialogueUI.Instance.EnterSlot(s.slot, sp, s.speaker);
            }
        }
    }


    private bool CheckCondition(DialogueNode n)
    {
        if (string.IsNullOrEmpty(n.condition_key))
            return true;

        return variables.Get(n.condition_key) == n.condition_value;
    }

    private string ParseText(string raw)
    {
        return Regex.Replace(raw ?? "", "{(.*?)}",
            m => variables.Get(m.Groups[1].Value));
    }

    private void LoadDialogueNodes(string path)
    {
        TextAsset csv = Resources.Load<TextAsset>(path);
        if (csv == null)
        {
            Debug.LogError($"Dialogue CSV NOT FOUND: {path}");
            return;
        }

        nodeTable.Clear();
        StringReader r = new StringReader(csv.text);
        bool header = true;

        while (true)
        {
            string line = r.ReadLine();
            if (line == null) break;
            if (header) { header = false; continue; }

            string[] c = ParseCSVLine(line);
            if (c.Length < 13) continue;

            DialogueNode n = new DialogueNode
            {
                id = ToInt(c[0]),
                scene_id = ToInt(c[1]),
                speaker = c[2].Trim(),
                emotion = ToInt(c[3]),
                text = Clean(c[4]),
                condition_key = c[5].Trim(),
                condition_value = c[6].Trim(),
                choice0_text = c[7].Trim(),
                choice0_next = ToInt(c[8]),
                command = c[9].Trim(),
                command_target = c[10].Trim(),
                command_value = c[11].Trim(),
                next_id = ToInt(c[12])
            };
            nodeTable[n.id] = n;
        }
    }

    private void LoadSceneSlots(string path)
    {
        TextAsset csv = Resources.Load<TextAsset>(path);
        if (csv == null) return;

        sceneSlotTable.Clear();
        StringReader r = new StringReader(csv.text);
        bool header = true;

        while (true)
        {
            string line = r.ReadLine();
            if (line == null) break;
            if (header) { header = false; continue; }

            string[] c = ParseCSVLine(line);
            if (c.Length < 5) continue;

            DialogueSceneSlot s = new DialogueSceneSlot
            {
                scene_id = ToInt(c[0]),
                slot = c[1].Trim(),
                speaker = c[2].Trim(),
                personality = ToInt(c[3]),
                emotion = ToInt(c[4])
            };

            if (!sceneSlotTable.ContainsKey(s.scene_id))
                sceneSlotTable[s.scene_id] = new List<DialogueSceneSlot>();

            sceneSlotTable[s.scene_id].Add(s);
        }
    }

    private int ToInt(string s) => int.TryParse(s, out var v) ? v : 0;
    private string Clean(string s) => s.Replace("\"\"\"", "").Replace("\"", "").Replace("\\n", "\n");

    private string[] ParseCSVLine(string line)
    {
        List<string> result = new();
        bool inQuotes = false;
        string cur = "";

        for (int i = 0; i < line.Length; i++)
        {
            char ch = line[i];

            if (ch == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    cur += '"';
                    i++;
                }
                else
                    inQuotes = !inQuotes;
            }
            else if (ch == ',' && !inQuotes)
            {
                result.Add(cur);
                cur = "";
            }
            else
                cur += ch;
        }

        result.Add(cur);
        return result.ToArray();
    }
}
