using UnityEngine;
using System.Collections.Generic;

public class DialogueLoader : MonoBehaviour
{
    public static DialogueLoader Instance;

    private Dictionary<int, PersonalityDialogue> dialogueDict;

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
        TextAsset jsonFile = Resources.Load<TextAsset>("Dialogues/personality_dialogue");

        if (jsonFile == null)
        {
            Debug.LogError("JSON 파일을 찾을 수 없습니다! 경로: Resources/Dialogues/personality_dialogue.json");
            return;
        }

        PersonalityDialogueCollection collection =
            JsonUtility.FromJson<PersonalityDialogueCollection>(jsonFile.text);

        dialogueDict = new Dictionary<int, PersonalityDialogue>();

        foreach (PersonalityDialogue p in collection.personalities)
        {
            if (!dialogueDict.ContainsKey(p.id))
                dialogueDict.Add(p.id, p);
            else
                Debug.LogWarning($"중복 ID 감지: {p.id}");
        }

        Debug.Log($"대사 JSON 로드 완료: {dialogueDict.Count}개");
    }

    public PersonalityDialogue Get(int id)
    {
        if (dialogueDict == null)
        {
            Debug.LogError("dialogueDict가 초기화되지 않았습니다. DialogueLoader가 존재하는지 확인하세요.");
            return null;
        }

        if (dialogueDict.TryGetValue(id, out var entry))
            return entry;

        Debug.LogWarning("해당 ID의 대사를 찾을 수 없습니다: " + id);
        return null;
    }
}
