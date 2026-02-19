using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using static UnityEngine.Rendering.DebugUI;
using System.Collections;
//using Palmmedia.ReportGenerator.Core;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    private Dictionary<int, DialogueNode> nodeTable = new();
    private Dictionary<int, List<DialogueSceneSlot>> sceneSlotTable = new();

    private int currentNodeId;
    private int currentSceneId = -1;
    private bool waitingForChoice = false;
    private bool autoStartFinished = false;

    //  [추가] "현재 대화 중인 NPC" 저장 (살해 타겟)
    public GameObject CurrentNpc { get; private set; }
    public void SetCurrentNpc(GameObject npc) => CurrentNpc = npc;

    private void Awake()
    {
        Instance = this;

        PortraitDatabase.LoadAll();

        nodeTable.Clear();

        LoadDialogueNodes("Dialogues/dialogue_Scene");
        LoadDialogueNodes("Dialogues/dialogue_ObjectScene");

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

            //  [추가] 대화 끝났으면 타겟 초기화(원하면 지워도 됨)
            CurrentNpc = null;
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
            if (DialogueUI.Instance != null)
                DialogueUI.Instance.HideCutScene();

            //  [추가] 대화 끝났으면 타겟 초기화(원하면 지워도 됨)
            CurrentNpc = null;
            return;
        }

        if (!nodeTable.TryGetValue(id, out var node))
        {
            currentNodeId = -1;
            DialogueUI.Instance.Close();

            //  [추가]
            CurrentNpc = null;
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

            //  [추가]
            CurrentNpc = null;
            return;
        }

        if (node.speaker == "Choice")
        {
            waitingForChoice = true;
            ShowChoiceBlock(id);
            return;
        }

        bool canContinue = true;

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
                canContinue = RunSystem(node);
            }
        }

        if (!canContinue)
            return;

        if (!string.IsNullOrEmpty(node.text))
        {
            DialogueUI.Instance.ShowLine(
                node.speaker,
                ParseText(node.text)
            );
        }

        if (node.next_id < 0)
        {
            currentNodeId = -1;
            return;
        }

        if (!nodeTable.TryGetValue(node.next_id, out var nextNode))
        {
            currentNodeId = -1;
            return;
        }

        if (nextNode.speaker == "System")
        {
            // fade_wait(in)은 자동 실행
            if (IsWaitingSystem(nextNode) && nextNode.command_target == "in")
            {
                ExecuteNode(node.next_id);
                return;
            }

            // 그 외 대기형 System은 입력 대기
            if (IsWaitingSystem(nextNode))
            {
                currentNodeId = node.next_id;
                return;
            }

            ExecuteNode(node.next_id);
            return;
        }

        if (nextNode.speaker == "Choice")
        {
            waitingForChoice = true;
            ShowChoiceBlock(node.next_id);
            return;
        }

        currentNodeId = node.next_id;
    }

    private bool IsWaitingSystem(DialogueNode node)
    {
        if (string.IsNullOrEmpty(node.command))
            return false;

        string cmd = node.command.Trim();
        return cmd == "fade_wait" || cmd == "delay";
    }

    private void ShowChoiceBlock(int startId)
    {
        List<(string text, Action onClick)> choices = new();
        int id = startId;

        while (nodeTable.TryGetValue(id, out var n)
               && n.scene_id == currentSceneId
               && n.speaker == "Choice")
        {
            int next = n.next_id;

            string cmd = n.command?.Trim();
            string tgt = n.command_target?.Trim();
            string val = n.command_value?.Trim();

            string text = n.text;

            choices.Add((text, () =>
            {
                waitingForChoice = false;

                //  "살해" 선택 처리
                if (!string.IsNullOrEmpty(text) && text.Trim() == "살해")
                {
                    var npc = CurrentNpc;
                    if (npc != null && npc.TryGetComponent<CultistKillable>(out var killable))
                    {
                        Debug.Log($"[CHOICE] '살해' selected. Target = {npc.name}");
                        killable.GriefKill();
                    }
                    else
                    {
                        Debug.LogWarning("[CHOICE] '살해' selected but CurrentNpc is null or not killable.");
                    }
                }

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

        
        // 여기부터가 "비애가 아니면 선택지 UI를 아예 안 띄움" 처리
       
        bool isGrief = false;
        if (PlayerPersonality.Instance != null)
        {
            isGrief = (PlayerPersonality.Instance.currentPersonality == PersonalityType.grief);
        }

        if (!isGrief)
        {
            // 비애가 아니면 자동으로 "자비" 선택(없으면 첫 선택지)
            waitingForChoice = false;

            int idx = choices.FindIndex(c => !string.IsNullOrEmpty(c.text) && c.text.Trim() == "자비");
            if (idx < 0) idx = 0;

            if (choices.Count > 0)
            {
                Debug.Log($"[CHOICE] auto-select (not grief): {choices[idx].text}");
                choices[idx].onClick?.Invoke();
            }
            else
            {
                // 선택지 없으면 그냥 종료
                ExecuteNode(-1);
            }
            return;
        }

        // 비애면 정상적으로 선택지 UI 띄움
        DialogueUI.Instance.ShowChoices(choices);
    }

    private bool RunSystem(DialogueNode node)
    {
        string cmd = node.command?.Trim();
        string target = node.command_target?.Trim();
        string value = node.command_value?.Trim();

        if (cmd == "move")
        {
            WorldMove(target, value);

            CameraFollow cam = Camera.main.GetComponent<CameraFollow>();
            if (cam != null)
            {
                GameObject obj = GameObject.Find(target);
                if (obj != null)
                    cam.SnapToPositionImmediate(obj.transform.position);
            }

            return true;
        }

        if (cmd == "fade")
        {
            if (FadeManager.Instance == null)
                return true;

            if (target == "out")
                FadeManager.Instance.StartCoroutine(FadeManager.Instance.FadeOut());
            else if (target == "in")
                FadeManager.Instance.StartCoroutine(FadeManager.Instance.FadeIn());

            return true;
        }

        if (cmd == "fade_wait")
        {
            if (FadeManager.Instance == null)
                return false;

            if (DialogueUI.Instance != null)
                DialogueUI.Instance.HideCutScene();
            // 페이드 시작 전 시간 정지
            Time.timeScale = 0f;

            CameraFollow cam = Camera.main.GetComponent<CameraFollow>();
            if (cam != null)
                cam.BeginExternalControl();

            if (target == "out")
            {
                if (DialogueUI.Instance != null)
                    DialogueUI.Instance.Close();

                FadeManager.Instance.StartCoroutine(
                    FadeManager.Instance.FadeOut(() =>
                    {
                        // 페이드 완료 후 시간 재개
                        Time.timeScale = 1f;

                        ExecuteNode(node.next_id);
                    })
                );
            }
            else if (target == "in")
            {
                FadeManager.Instance.StartCoroutine(
                    FadeManager.Instance.FadeIn(() =>
                    {
                        if (cam != null)
                            cam.EndExternalControl();

                        // 페이드 완료 후 시간 재개
                        Time.timeScale = 1f;

                        ExecuteNode(node.next_id);
                    })
                );
            }

            return false;
        }

        if (cmd == "walk")
        {
            StartCoroutine(WalkAndNext(target, value, node.next_id));
            return false;
        }

        if (cmd == "set_next")
        {
            GameObject targetObj = GameObject.Find(target);
            if (targetObj == null)
            {
                Debug.LogWarning($"[Dialogue] set_next 대상 없음: {target}");
                return true;
            }

            var NPC = targetObj.GetComponent<NPCDialogueTrigger>();

            if (NPC != null)
            {
                int nextId = 0;
                int.TryParse(value, out nextId);

                NPC.startSceneId = nextId;
                if (nextId == 1)
                {
                    NPC.doneFlagKey = "dl_done";
                }
                else
                {
                    NPC.doneFlagKey = "dl_done" + (nextId - 1);
                }

                Debug.Log($"[Dialogue] set_next 대상 설정: {target} -> scene {nextId},  flag {NPC.doneFlagKey}");
            }
            else
            {
                Debug.LogWarning($"[Dialogue] set_next 대상에 NPCDialogueTrigger 없음: {target}");
            }
            return true;
        }

        if (cmd == "delay")
        {
            float sec = 0f;
            float.TryParse(value, out sec);

            StartCoroutine(DelayAndNext(sec, node.next_id));
            return false;
        }

        if (cmd == "revive")
        {
            GameObject targetObj = GameObject.Find(target);
            if (targetObj == null)
            {
                Debug.LogWarning($"[Dialogue] revive 대상 없음: {target}");
                return true;
            }

            var deathState = targetObj.GetComponent<NPCDeathState>();
            if (deathState != null)
            {
                deathState.Revive();
            }
            else
            {
                Debug.LogWarning($"[Dialogue] {target}에 NPCDeathState 스크립트가 없습니다.");
            }

            return true;
        }

        if (cmd == "cutscene")
        {
            if (DialogueUI.Instance != null)
            {
                int index = 0;
                int.TryParse(target, out index);
                DialogueUI.Instance.ShowCutScene(index);
            }

            return true;
        }

        if (cmd == "set_active")
        {
            bool isActive = false;
            if (value != null && (value.ToLower() == "true" || value == "1"))
            {
                isActive = true;
            }

            GameObject targetObj = FindObjectAnywhere(target);

            if (targetObj != null)
            {
                targetObj.SetActive(isActive);
                Debug.Log($"[Dialogue] {target} Active 상태 변경: {isActive}");
                if (isActive)
                {
                    Transform parent = targetObj.transform.parent;
                    while (parent != null)
                    {
                        if (!parent.gameObject.activeSelf)
                        {
                            parent.gameObject.SetActive(true);
                        }
                        parent = parent.parent;
                    }
                }
            }
            else
            {
                Debug.LogWarning($"[Dialogue] set_active 실패: '{target}' 오브젝트를 찾을 수 없습니다.");
            }

            return true;
        }

        return true;
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

    private void WorldMove(string characterName, string pointKey)
    {
        GameObject targetObj = GameObject.Find(characterName);
        if (targetObj == null)
        {
            Debug.LogWarning($"[Dialogue] WorldMove 대상 없음: {characterName}");
            return;
        }

        Transform point = WorldMovePointTable.Instance?.Get(pointKey);
        if (point == null)
        {
            Debug.LogWarning($"[Dialogue] 이동 위치 없음: {pointKey}");
            return;
        }

        targetObj.transform.position = point.position;
    }

    private IEnumerator DelayAndNext(float sec, int nextId)
    {
        float t = 0f;
        while (t < sec)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        ExecuteNode(nextId);
    }

    public void RequestNext()
    {
        // Choice 중이면 입력 자체를 무시
        if (waitingForChoice)
            return;

        Next();
    }

    private IEnumerator WalkAndNext(string characterName, string pointKey, int nextId)
    {
        if (DialogueUI.Instance != null)
        {
            DialogueUI.Instance.Close();
        }
        //걸을때 시간 정지
        Time.timeScale = 0f;

        GameObject targetObj = GameObject.Find(characterName);
        if (targetObj == null)
        {
            Debug.LogWarning($"[Dialogue] Walk 대상 없음: {characterName}");
            ExecuteNode(nextId);
            yield break;
        }

        Transform point = WorldMovePointTable.Instance?.Get(pointKey);
        Vector3 destPos = (point != null) ? point.position : targetObj.transform.position;
        destPos.z = targetObj.transform.position.z;

        // 컴포넌트 가져오기, 제어권 뺏기
        var playerMove = targetObj.GetComponent<PlayerMove>();
        var anim = targetObj.GetComponent<Animator>();
        var sr = targetObj.GetComponent<SpriteRenderer>();

        if (playerMove != null) playerMove.enabled = false;

        float speed = (playerMove != null) ? playerMove.moveSpeed : 3f;

        while (Vector3.Distance(targetObj.transform.position, destPos) > 0.01f)
        {
            Vector3 dir = (destPos - targetObj.transform.position).normalized;

            if (anim != null) anim.SetBool("IsWalk", true);

            if (sr != null)
            {
                if (dir.x > 0) sr.flipX = true;
                else if (dir.x < 0) sr.flipX = false;
            }

            targetObj.transform.position = Vector3.MoveTowards(
                targetObj.transform.position,
                destPos,
                speed * Time.unscaledDeltaTime
            );

            yield return null;
        }

        targetObj.transform.position = destPos; // 위치 보정

        if (anim != null) anim.SetBool("IsWalk", false); // 걷기 모션 해제
        if (playerMove != null) playerMove.enabled = true; // 제어권 반환

        Time.timeScale = 1f; // 시간 재개

        ExecuteNode(nextId);
    }

    private GameObject FindObjectAnywhere(string name)
    {
        GameObject obj = GameObject.Find(name);
        if (obj != null) return obj;

        //씬 내 전체 탐색, 대화때만 사용이라 성능 신경 안씀
        foreach (GameObject root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (root.name == name) return root;

            Transform result = RecursiveFindChild(root.transform, name);
            if (result != null) return result.gameObject;
        }

        return null;
    }

    private Transform RecursiveFindChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child;

            Transform result = RecursiveFindChild(child, name);
            if (result != null) return result;
        }
        return null;
    }
}
