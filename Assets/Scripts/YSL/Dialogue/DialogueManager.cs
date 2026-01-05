using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using System.IO;
using System.Linq;
using Unity.VisualScripting;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    private Dictionary<int, DialogueNode> nodeTable = new();
    private Dictionary<int, List<DialogueSceneSlot>> sceneSlotTable = new();

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
    public bool CurrentNodeIsValid()
    {
        return currentNodeId >= 0;
    }

    public void StartDialogueByScene(int sceneId)
    {
        Debug.Log($"[StartDialogueByScene] sceneId={sceneId}, dl_done0={DialogueFlagTable.Instance?.Get("dl_done0")}");

        // 완전 초기화
        waitingForChoice = false;
        currentNodeId = -1;
        currentSceneId = -1;

        if (DialogueUI.Instance != null)
            DialogueUI.Instance.Close();

        DialogueNode startNode = nodeTable.Values
            .Where(n => n.scene_id == sceneId)
            .OrderBy(n => n.id)
            .FirstOrDefault();

        if (startNode == null)
            return;

        // 조건 실패 시 아무 것도 하지 않음
        if (!CheckCondition(startNode))
            return;

        UIManager.Instance.Open(DialogueUI.Instance);
        ExecuteNode(startNode.id);
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
            currentNodeId = -1;
            DialogueUI.Instance.Close();
            return;
        }

        if (!nodeTable.TryGetValue(id, out var node))
        {
            currentNodeId = -1;
            DialogueUI.Instance.Close();
            return;
        }

        currentNodeId = id;

        // scene 변경 시 초기 슬롯 세팅
        if (node.scene_id != currentSceneId)
        {
            ApplyScene(node.scene_id);
            currentSceneId = node.scene_id;
        }

        // 조건 실패 시 종료
        if (!CheckCondition(node))
        {
            currentNodeId = -1;
            DialogueUI.Instance.Close();
            return;
        }

        // Choice 처리
        if (node.speaker == "Choice")
        {
            waitingForChoice = true;
            ShowChoiceBlock(id);
            return;
        }

        // emotion 변경 반영 (기존 API만 사용)
        if (!string.IsNullOrEmpty(node.speaker))
        {
            string targetSlot = null;

            foreach (string slot in DialogueUI.Instance.GetAllSlots())
            {
                string speakerInSlot = DialogueUI.Instance.GetSpeakerInSlot(slot);
                if (!string.IsNullOrEmpty(speakerInSlot) &&
                    speakerInSlot == node.speaker)
                {
                    targetSlot = slot;
                    break;
                }
            }

            if (!string.IsNullOrEmpty(targetSlot))
            {
                Sprite sp = null;

                if (node.speaker == "라즐리")
                {
                    sp = PortraitDatabase.GetPlayer(0, node.emotion);
                }
                else
                {
                    sp = PortraitDatabase.GetNPC(node.speaker);
                }

                if (sp != null)
                {
                    DialogueUI.Instance.EnterSlot(
                        targetSlot,
                        sp,
                        node.speaker
                    );
                }
            }
        }

        // command 처리
        if (!string.IsNullOrEmpty(node.command))
        {
            if (node.command == "set_var")
            {
                DialogueFlagTable.Instance?.Set(
                    node.command_target,
                    node.command_value
                );
            }
            else
            {
                RunSystem(node);
            }
        }

        // 대사 출력
        DialogueUI.Instance.ShowLine(
            node.speaker,
            ParseText(node.text)
        );

        // 종료 처리
        if (node.next_id < 0)
        {
            currentNodeId = -1;
            return;
        }

        currentNodeId = node.next_id;
    }

    private void ShowChoiceBlock(int startId)
    {
        List<(string, Action)> choices = new();
        int id = startId;

        while (nodeTable.TryGetValue(id, out var n)
               && n.scene_id == currentSceneId
               && n.speaker == "Choice")
        {
            int next = n.next_id; // 핵심: choice0_next 사용 금지

            string cmd = n.command?.Trim();
            string tgt = n.command_target?.Trim();
            string val = n.command_value?.Trim();

            string text = n.text;

            choices.Add((text, () =>
            {
                waitingForChoice = false;

                if (cmd == "set_var")
                {
                    DialogueFlagTable.Instance?.Set(tgt, val);
                }

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

        if (DialogueFlagTable.Instance == null)
            return false;

        var field = typeof(DialogueFlagTable)
            .GetField(n.condition_key);

        if (field == null || field.FieldType != typeof(bool))
        {
            Debug.LogWarning(
                $"[Dialogue] Condition key '{n.condition_key}' not found in DialogueFlagTable"
            );
            return false;
        }

        bool flagValue = (bool)field.GetValue(DialogueFlagTable.Instance);
        bool expected = bool.Parse(n.condition_value);

        return flagValue == expected;
    }


    private string ParseText(string raw)
    {
        return Regex.Replace(raw ?? "", "{(.*?)}", m =>
        {
            if (DialogueFlagTable.Instance == null)
                return "";

            string key = m.Groups[1].Value;
            string v = DialogueFlagTable.Instance.Get(key);
            return string.IsNullOrEmpty(v) ? "" : v;
        });
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

            // 컬럼 수 검증
            if (c.Length < 13)
            {
                Debug.LogError($"[CSV ERROR] column count={c.Length} line={line}");
                continue;
            }

            DialogueNode n = new DialogueNode
            {
                id = ToInt(Normalize(c[0])),
                scene_id = ToInt(Normalize(c[1])),
                speaker = Normalize(c[2]),
                emotion = ToInt(Normalize(c[3])),
                text = Clean(c[4]),

                condition_key = Normalize(c[5]),
                condition_value = Normalize(c[6]),

                choice0_text = Normalize(c[7]),
                choice0_next = ToInt(Normalize(c[8])),

                command = Normalize(c[9]),
                command_target = Normalize(c[10]),
                command_value = Normalize(c[11]),

                next_id = ToInt(Normalize(c[12]))
            };

            nodeTable[n.id] = n;

            // 핵심 로그
            if (n.scene_id == 1)
            {
                Debug.Log($"[LOADED scene=1] id={n.id}, speaker={n.speaker}, condKey={n.condition_key}, condVal={n.condition_value}");
            }
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


    private string Normalize(string s)
    {
        if (string.IsNullOrEmpty(s))
            return "";

        return s
            .Replace("\uFEFF", "")   // BOM 제거 (핵심)
            .Trim();
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
