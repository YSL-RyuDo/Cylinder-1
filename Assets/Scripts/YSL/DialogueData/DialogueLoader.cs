using System.Collections.Generic;
using UnityEngine;

public class DialogueLoader : MonoBehaviour
{
    public static DialogueLoader Instance { get; private set; }

    private Dictionary<int, List<List<string>>> portalDialogueDict = new Dictionary<int, List<List<string>>>();

    void Awake()
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
        TextAsset jsonText = Resources.Load<TextAsset>("PortalLockedDialogues");
        if (jsonText == null)
        {
            Debug.LogError("PortalLockedDialogues.json 파일을 Resources 폴더에 넣으세요.");
            return;
        }

        DialogueDatas data = JsonUtility.FromJson<DialogueDatas>(jsonText.text);
        foreach (var dlg in data.portal_locked_dialogues)
        {
            var lineList = new List<List<string>>();
            foreach (var line in dlg.lines)
            {
                lineList.Add(line.segments);
            }
            portalDialogueDict[dlg.personality_id] = lineList;
        }
    }

    public string[] GetRandomPortalDialogueLines(int personalityId)
    {
        if (portalDialogueDict.TryGetValue(personalityId, out var allLines) && allLines.Count > 0)
        {
            var chosen = allLines[Random.Range(0, allLines.Count)];
            return chosen.ToArray();
        }
        return new string[] { "..." };
    }
}
