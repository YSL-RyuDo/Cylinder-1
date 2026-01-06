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
    private bool autoStartFinished = false;

    private void Awake()
    {
        Instance = this;

        PortraitDatabase.LoadAll();
        LoadDialogueNodes("Dialogues/dialogue_Scene");
        LoadSceneSlots("Dialogues/dialogue");
    }
    private void Start()
    {
        if (autoStartFinished)
            return;

        if (DialogueFlagTable.Instance == null)
            return;

        if (!DialogueFlagTable.Instance.dl_done)
        {
            autoStartFinished = true;
            StartDialogueByScene(0);
        }
    }

    public bool CurrentNodeIsValid()
    {
        return currentNodeId >= 0;
    }

    public void StartDialogueByScene(int sceneId)
    {
        // scene 0은 한 번 끝나면 다시 시작 금지
        if (sceneId == 0 &&
            DialogueFlagTable.Instance != null &&
            DialogueFlagTable.Instance.dl_done)
        {
            return;
        }

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

        if (!CheckCondition(startNode))
            return;

        UIManager.Instance.Open(DialogueUI.Instance);
        ExecuteNode(startNode.id);
    }

    public void Next()
    {
        // 타이핑 중이면 다음 노드로 가지 말고 텍스트 완성
        if (DialogueUI.Instance.IsTyping())
        {
            DialogueUI.Instance.ForceCompleteTyping();
            return;
        }

        if (waitingForChoice)
            return;

        if (currentNodeId < 0)
        {
            DialogueUI.Instance.Close();
            CheckFinalDialogueCondition();
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
            CheckFinalDialogueCondition();
            return;
        }

        if (!nodeTable.TryGetValue(id, out var node))
        {
            currentNodeId = -1;
            DialogueUI.Instance.Close();
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
            currentNodeId = -1;
            DialogueUI.Instance.Close();
            return;
        }

        // Choice는 즉시 처리
        if (node.speaker == "Choice")
        {
            waitingForChoice = true;
            ShowChoiceBlock(id);
            return;
        }

        // System / set_var 먼저 처리 (중요)
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

        // 텍스트는 반드시 출력
        if (!string.IsNullOrEmpty(node.text))
        {
            DialogueUI.Instance.ShowLine(
                node.speaker,
                ParseText(node.text)
            );
        }

        // 종료 노드
        if (node.next_id < 0)
        {
            currentNodeId = -1;
            return;
        }

        // 다음 노드 판별
        if (!nodeTable.TryGetValue(node.next_id, out var nextNode))
        {
            currentNodeId = -1;
            return;
        }

        // 다음이 System이면 자동 연속 실행
        if (nextNode.speaker == "System")
        {
            ExecuteNode(node.next_id);
            return;
        }

        // 다음이 Choice면 즉시 Choice 표시
        if (nextNode.speaker == "Choice")
        {
            waitingForChoice = true;
            ShowChoiceBlock(node.next_id);
            return;
        }

        // 그 외는 입력 대기
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

        if (cmd == "enter")
        {
            Sprite sp = null;

            if (target == "라즐리")
                sp = PortraitDatabase.GetPlayer(0, 0);
            else
                sp = PortraitDatabase.GetNPC(target);

            if (sp == null)
                return;

            DialogueUI.Instance.EnterSlot(node.command_value, sp, target);
            return;
        }

        if (cmd == "move")
        {
            Sprite foundSprite = null;

            foreach (string slot in DialogueUI.Instance.GetAllSlots())
            {
                string speaker = DialogueUI.Instance.GetSpeakerInSlot(slot);
                if (speaker == target)
                {
                    foundSprite = DialogueUI.Instance.GetSlotSprite(slot);
                    break;
                }
            }

            if (foundSprite == null)
                return;

            DialogueUI.Instance.EnterSlot(node.command_value, foundSprite, target);
            return;
        }

        if (cmd == "fade")
        {
            if (FadeManager.Instance == null)
                return;

            if (target == "out")
            {
                FadeManager.Instance.StartCoroutine(
                    FadeManager.Instance.FadeOut()
                );
            }
            else if (target == "in")
            {
                FadeManager.Instance.StartCoroutine(
                    FadeManager.Instance.FadeIn()
                );
            }

            return;
        }
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

    private void CheckFinalDialogueCondition()
    {
        // 대화 중이면 무시
        if (currentNodeId >= 0)
            return;

        var f = DialogueFlagTable.Instance;
        if (f == null)
            return;

        // scene 0이 아직 끝나지 않았으면 실행 금지
        if (!f.dl_done)
            return;

        // 이미 최종 대화 실행했으면 무시
        if (f.dl_done5)
            return;

        if (f.dl_done0 &&
            f.dl_done1 &&
            f.dl_done2 &&
            f.dl_done3 &&
            f.dl_done4)
        {
            StartDialogueByScene(6);
        }
    }

}
