using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;

public class DialogueUI : MonoBehaviour, IUIWindow
{
    public static DialogueUI Instance;

    [SerializeField] private float typingSpeed = 0.04f;

    private Coroutine typingCoroutine;
    private bool isTyping;
    private string fullText;

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
    private Dictionary<string, string> slotSpeakerMap = new();
    private readonly List<Button> spawnedChoices = new();

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
        if (IsOpen) return;
        dialogueRoot.SetActive(true);
        IsOpen = true;
    }

    public void Close()
    {
        if (!IsOpen) return;

        StopTypingImmediate();
        dialogueRoot.SetActive(false);
        ClearChoices();
        ClearSlots();
        ClearSlotSpeakers();

        IsOpen = false;
        UIManager.Instance.NotifyWindowClosed(this);
    }

    public void ShowLine(string speaker, string text)
    {
        Open();
        ClearChoices();
        StopTypingImmediate();

        speakerText.text = "";
        dialogueText.text = "";
        dialogueText.gameObject.SetActive(true);

        if (speaker == "Narration")
        {
            backgroundImage.sprite = narrationBackground;
            speakerText.gameObject.SetActive(false);
            dialogueText.color = Color.white;
        }
        else
        {
            backgroundImage.sprite = dialogueBackground;
            speakerText.gameObject.SetActive(true);
            speakerText.text = speaker;
            dialogueText.color = Color.black;
        }

        fullText = text ?? "";
        isTyping = true;
        typingCoroutine = StartCoroutine(TypeTextCoroutine());
    }

    private IEnumerator TypeTextCoroutine()
    {
        dialogueText.text = "";

        for (int i = 0; i < fullText.Length; i++)
        {
            dialogueText.text += fullText[i];
            yield return new WaitForSecondsRealtime(typingSpeed);
        }

        isTyping = false;
        typingCoroutine = null;
    }

    public bool IsTyping()
    {
        return isTyping;
    }

    public void ForceCompleteTyping()
    {
        if (!isTyping) return;

        StopTypingImmediate();
        dialogueText.text = fullText;
    }

    private void StopTypingImmediate()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
        isTyping = false;
    }

    public void ShowChoices(List<(string text, Action onClick)> choices)
    {
        Open();
        ClearChoices();

        foreach (var c in choices)
        {
            Button btn = Instantiate(choiceButtonPrefab, choiceContainer);
            btn.GetComponentInChildren<TMP_Text>().text = c.text;

            btn.onClick.AddListener(() =>
            {
                StopTypingImmediate();
                c.onClick?.Invoke();
            });

            spawnedChoices.Add(btn);
        }
    }

    private void ClearChoices()
    {
        foreach (var b in spawnedChoices)
            if (b != null) Destroy(b.gameObject);
        spawnedChoices.Clear();
    }

    public void EnterSlot(string slotId, Sprite portrait, string speaker)
    {
        if (!slotTable.TryGetValue(slotId, out var img)) return;
        img.sprite = portrait;
        img.gameObject.SetActive(true);
        slotSpeakerMap[slotId] = speaker;
    }

    public void ClearSlots()
    {
        foreach (var img in slotTable.Values)
            img.gameObject.SetActive(false);
    }

    public IEnumerable<string> GetAllSlots() => slotTable.Keys;

    public Sprite GetSlotSprite(string slotId)
    {
        if (!slotTable.TryGetValue(slotId, out var img)) return null;
        return img.gameObject.activeSelf ? img.sprite : null;
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
