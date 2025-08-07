using System.Collections.Generic;
using UnityEngine;

public class DialogueLoader : MonoBehaviour
{
    public static DialogueLoader Instance { get; private set; }

    private Dictionary<int, List<List<string>>> portalLockedDialogueDict = new Dictionary<int, List<List<string>>>();
    private Dictionary<int, List<List<string>>> portalUnlockedDialogueDict = new Dictionary<int, List<List<string>>>();
    private Dictionary<int, List<List<string>>> personalitySwitchFailDialogueDict = new Dictionary<int, List<List<string>>>();
    private Dictionary<int, List<List<string>>> personalitySwitchSuccessDialogueDict = new Dictionary<int, List<List<string>>>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadDialogueJSON();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadDialogueJSON()
    {
        // 잠긴 대사 로드
        TextAsset lockedJsonText = Resources.Load<TextAsset>("PortalLockedDialogues");
        if (lockedJsonText == null)
        {
            Debug.LogError("PortalLockedDialogues.json 파일을 Resources 폴더에 넣으세요.");
        }
        else
        {
            DialogueDatas lockedData = JsonUtility.FromJson<DialogueDatas>(lockedJsonText.text);
            if (lockedData.portal_locked_dialogues != null)
            {
                foreach (var dlg in lockedData.portal_locked_dialogues)
                {
                    var lineList = new List<List<string>>();
                    foreach (var line in dlg.lines)
                    {
                        lineList.Add(line.segments);
                    }
                    portalLockedDialogueDict[dlg.personality_id] = lineList;
                }
            }
        }

        // 열린 대사 로드
        TextAsset unlockedJsonText = Resources.Load<TextAsset>("PortalUnlockedDialogues");
        if (unlockedJsonText == null)
        {
            Debug.LogError("PortalUnlockedDialogues.json 파일을 Resources 폴더에 넣으세요.");
        }
        else
        {
            DialogueDatas unlockedData = JsonUtility.FromJson<DialogueDatas>(unlockedJsonText.text);
            if (unlockedData.portal_unlocked_dialogues != null)
            {
                foreach (var dlg in unlockedData.portal_unlocked_dialogues)
                {
                    var lineList = new List<List<string>>();
                    foreach (var line in dlg.lines)
                    {
                        lineList.Add(line.segments);
                    }
                    portalUnlockedDialogueDict[dlg.personality_id] = lineList;
                }
            }
        }

        // 실패 대사 로드
        TextAsset failJsonText = Resources.Load<TextAsset>("PersonalitySwitchFailDialogues");
        if (failJsonText == null)
        {
            Debug.LogError("PersonalitySwitchFailDialogues.json 파일을 Resources 폴더에 넣으세요.");
        }
        else
        {
            DialogueDatas failData = JsonUtility.FromJson<DialogueDatas>(failJsonText.text);
            if (failData.personality_switch_fail_dialogues != null)
            {
                foreach (var dlg in failData.personality_switch_fail_dialogues)
                {
                    var lineList = new List<List<string>>();
                    foreach (var line in dlg.lines)
                    {
                        lineList.Add(line.segments);
                    }
                    personalitySwitchFailDialogueDict[dlg.personality_id] = lineList;
                }
            }
        }

        // 성공 대사 로드 (추가)
        TextAsset successJsonText = Resources.Load<TextAsset>("PersonalitySwitchSuccessDialogues");
        if (successJsonText == null)
        {
            Debug.LogError("PersonalitySwitchSuccessDialogues.json 파일을 Resources 폴더에 넣으세요.");
        }
        else
        {
            DialogueDatas successData = JsonUtility.FromJson<DialogueDatas>(successJsonText.text);
            if (successData.personality_switch_success_dialogues != null)
            {
                foreach (var dlg in successData.personality_switch_success_dialogues)
                {
                    var lineList = new List<List<string>>();
                    foreach (var line in dlg.lines)
                    {
                        lineList.Add(line.segments);
                    }
                    personalitySwitchSuccessDialogueDict[dlg.personality_id] = lineList;
                }
            }
        }
    }


    // 잠긴 포탈 대사 랜덤 추출
    public string[] GetRandomLockedPortalDialogueLines(int personalityId)
    {
        if (portalLockedDialogueDict.TryGetValue(personalityId, out var allLines) && allLines.Count > 0)
        {
            var chosen = allLines[Random.Range(0, allLines.Count)];
            return chosen.ToArray();
        }
        return new string[] { "..." };
    }

    // 열린 포탈 대사 랜덤 추출
    public string[] GetRandomUnlockedPortalDialogueLines(int personalityId)
    {
        if (portalUnlockedDialogueDict.TryGetValue(personalityId, out var allLines) && allLines.Count > 0)
        {
            var chosen = allLines[Random.Range(0, allLines.Count)];
            return chosen.ToArray();
        }
        return new string[] { "..." };
    }

    // 인격 전환 실패 대사 랜덤 추출 (추가)
    public string[] GetRandomPersonalitySwitchFailDialogueLines(int personalityId)
    {
        if (personalitySwitchFailDialogueDict.TryGetValue(personalityId, out var allLines) && allLines.Count > 0)
        {
            var chosen = allLines[Random.Range(0, allLines.Count)];
            return chosen.ToArray();
        }
        return new string[] { "..." };
    }

    // 인격 전환 성공 대사 랜덤 추출 (추가)
    public string[] GetRandomPersonalitySwitchSuccessDialogueLines(int personalityId)
    {
        if (personalitySwitchSuccessDialogueDict.TryGetValue(personalityId, out var allLines) && allLines.Count > 0)
        {
            var chosen = allLines[Random.Range(0, allLines.Count)];
            return chosen.ToArray();
        }
        return new string[] { "..." };
    }

}
