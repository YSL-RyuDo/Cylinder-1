using UnityEngine;
using System.Collections.Generic;

public class DialogueLoader : MonoBehaviour
{
    public static DialogueLoader Instance;

    private Dictionary<int, PersonalityDialogue> personalitydialogueDict;
    private Dictionary<int, PortalMonologue> portalmonologueDict;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadDialogueJSON();
    }

    private void LoadDialogueJSON()
    {
        TextAsset personalityJsonFile = Resources.Load<TextAsset>("Dialogues/personality_dialogue");
        PersonalityDialogueCollection personalityJsonCollection =
            JsonUtility.FromJson<PersonalityDialogueCollection>(personalityJsonFile.text);
        personalitydialogueDict = new Dictionary<int, PersonalityDialogue>();

        foreach (PersonalityDialogue p in personalityJsonCollection.personalities)
        {
            if (!personalitydialogueDict.ContainsKey(p.id))
                personalitydialogueDict.Add(p.id, p);
            else
                Debug.LogWarning($"중복 ID 감지: {p.id}");
        }

        TextAsset portalJsonFile = Resources.Load<TextAsset>("Dialogues/portal_monologue");
        PortalMonologueCollection portalCollection =
            JsonUtility.FromJson<PortalMonologueCollection>(portalJsonFile.text);
        portalmonologueDict = new Dictionary<int, PortalMonologue>();

        foreach (var m in portalCollection.portalMonologues)
        {
            if (!portalmonologueDict.ContainsKey(m.id))
                portalmonologueDict.Add(m.id, m);
        }

    }

    public PersonalityDialogue PersonalityGet(int id)
    {
        if (personalitydialogueDict == null)
        {
            Debug.LogError("dialogueDict가 초기화되지 않았습니다. DialogueLoader가 존재하는지 확인하세요.");
            return null;
        }

        if (personalitydialogueDict.TryGetValue(id, out var entry))
            return entry;

        Debug.LogWarning("해당 ID의 대사를 찾을 수 없습니다: " + id);
        return null;
    }

    public PortalMonologue PortalGet(int id)
    {
        if (portalmonologueDict.TryGetValue(id, out var data))
            return data;

        Debug.LogWarning($"해당 포탈 ID 대사 없음 : {id}");
        return null;
    }
}

