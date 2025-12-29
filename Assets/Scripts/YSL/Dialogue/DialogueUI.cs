using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class DialogueUI : MonoBehaviour, IUIWindow
{
    public static DialogueUI Instance;

    [Serializable]
    public class Slot
    {
        public string slotId;
        public Image portrait;
    }

    [Header("Root")]
    public GameObject dialogueRoot;

    [Header("Background")]
    public Image backgroundImage;
    public Sprite dialogueBackground;
    public Sprite narrationBackground;

    [Header("Text")]
    public TMP_Text speakerText;
    public TMP_Text dialogueText;

    [Header("Choices")]
    public Transform choiceContainer;
    public Button choiceButtonPrefab;

    [Header("Slots")]
    public List<Slot> slots = new();

    private Dictionary<string, Image> slotTable = new();
    private readonly List<Button> spawnedChoices = new();
    private Dictionary<string, string> slotSpeakerMap = new();

    public bool IsOpen { get; private set; }

    private void Awake()
    {
        Instance = this;

        slotTable.Clear();
        foreach (var s in slots)
        {
            if (s != null && s.portrait != null)
            {
                slotTable[s.slotId] = s.portrait;
                s.portrait.gameObject.SetActive(false);
            }
        }

        dialogueRoot.SetActive(false);
        ClearChoices();
        IsOpen = false;
    }

    public void Open()
    {
        if (IsOpen)
            return;

        dialogueRoot.SetActive(true);
        IsOpen = true;
        ///Time.timeScale = 0f;
        //Cursor.visible = true;
        //Cursor.lockState = CursorLockMode.None;
    }

    public void Close()
    {
        if (!IsOpen)
            return;

        dialogueRoot.SetActive(false);
        ClearChoices();
        ClearSlots();

        IsOpen = false;
        UIManager.Instance.NotifyWindowClosed(this);
    }

    public void ShowLine(string speaker, string text)
    {
        Debug.Log($"[ShowLine] CALLED | speaker='{speaker}' | text='{text}'");

        Debug.Log($"[ShowLine] Instance = {this.gameObject.name}, activeSelf={gameObject.activeSelf}");

        Open();

        Debug.Log($"[ShowLine] AFTER Open | dialogueRoot.activeSelf={dialogueRoot.activeSelf}");

        ClearChoices();

        Debug.Log($"[ShowLine] speakerText ref = {(speakerText == null ? "NULL" : speakerText.name)}");
        Debug.Log($"[ShowLine] dialogueText ref = {(dialogueText == null ? "NULL" : dialogueText.name)}");

        speakerText.text = "";
        dialogueText.text = "";

        dialogueText.gameObject.SetActive(true);

        if (speaker == "Narration")
        {
            Debug.Log("[ShowLine] Narration branch");

            backgroundImage.sprite = narrationBackground;
            speakerText.gameObject.SetActive(false);
            dialogueText.color = Color.white;
        }
        else
        {
            Debug.Log("[ShowLine] Dialogue branch");

            backgroundImage.sprite = dialogueBackground;
            speakerText.gameObject.SetActive(true);
            speakerText.text = speaker;
            dialogueText.color = Color.black;
            Debug.Log($"[ShowLine] speakerText.text SET = '{speakerText.text}'");
        }

        dialogueText.text = text;
        Debug.Log($"[ShowLine] dialogueText.text SET = '{dialogueText.text}'");
    }

    public void ShowChoices(List<(string text, Action onClick)> choices)
    {
        Debug.Log($"[ShowChoices] called, count = {choices.Count}");

        Open();
        ClearChoices();

        int index = 0;
        foreach (var c in choices)
        {
            int capturedIndex = index;

            Button btn = Instantiate(choiceButtonPrefab, choiceContainer);
            btn.GetComponentInChildren<TMP_Text>().text = c.text;

            btn.onClick.AddListener(() =>
            {
                Debug.Log($"[ChoiceButton CLICKED] index={capturedIndex}, text={c.text}");
                c.onClick();
            });

            spawnedChoices.Add(btn);
            index++;
        }
    }


    public void EnterSlot(string slotId, Sprite portrait, string speaker)
    {
        if (!slotTable.TryGetValue(slotId, out var img))
            return;

        img.sprite = portrait;
        img.gameObject.SetActive(true);

        slotSpeakerMap[slotId] = speaker;
    }

    public void RemoveSpeaker(string speakerName)
    {
        foreach (var pair in slotTable)
        {
            Image img = pair.Value;
            if (!img.gameObject.activeSelf)
                continue;

            if (img.sprite != null && img.sprite.name.Contains(speakerName))
            {
                img.gameObject.SetActive(false);
                img.sprite = null;
            }
        }
    }

    public void ClearSlots()
    {
        foreach (var img in slotTable.Values)
            img.gameObject.SetActive(false);
    }

    public Sprite GetSlotSprite(string slotId)
    {
        if (!slotTable.TryGetValue(slotId, out var img))
            return null;

        return img.gameObject.activeSelf ? img.sprite : null;
    }

    public IEnumerable<string> GetAllSlots()
    {
        return slotTable.Keys;
    }


    private void ClearChoices()
    {
        foreach (var b in spawnedChoices)
            Destroy(b.gameObject);

        spawnedChoices.Clear();
    }

    public string GetSpeakerInSlot(string slotId)
    {
        return slotSpeakerMap.TryGetValue(slotId, out var s) ? s : null;
    }

    public void ClearSlotSpeakers()
    {
        slotSpeakerMap.Clear();
    }
}
