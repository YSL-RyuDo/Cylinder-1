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

    [Header("Dialogue Window (대화창 패널)")]
    //  [추가] 대화창(배경+텍스트)이 묶여있는 패널 오브젝트를 넣어줘
    // (선택지 영역은 포함하지 않는 게 좋음)
    public GameObject dialogueWindowRoot;

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

    [Header("CutScene")]
    public Image cutSceneImage;
    public GameObject cutSceneBackGround;
    public GameObject cutSceneRoot;
    private Coroutine cutSceneFadeCoroutine;

    [Header("Next Button")]
    public Button nextButton;

    [Header("Slots")]
    public List<Slot> slots = new();

    private Dictionary<string, Image> slotTable = new();
    private Dictionary<string, string> slotSpeakerMap = new();
    private readonly List<Button> spawnedChoices = new();

    [Header("Transparency")]
    [SerializeField] private CanvasGroup panelCanvasGroup;

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

        if (nextButton != null)
        {
            nextButton.onClick.AddListener(() =>
            {
                DialogueManager.Instance?.RequestNext();
            });
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

    //  [추가] 선택지 전용: 대화창(프레임/배경) 자체를 숨긴다
    public void EnterChoiceMode()
    {
        StopTypingImmediate();

        // 대화창 패널 숨김 (배경/프레임까지 포함)
        if (dialogueWindowRoot != null)
            dialogueWindowRoot.SetActive(false);

        // 텍스트도 안전하게 비움
        if (speakerText != null) speakerText.text = "";
        if (dialogueText != null) dialogueText.text = "";

        // Next 버튼 숨김
        if (nextButton != null)
            nextButton.gameObject.SetActive(false);
    }

    //  [추가] 다시 대화 모드로 복귀
    public void ExitChoiceMode()
    {
        if (dialogueWindowRoot != null)
            dialogueWindowRoot.SetActive(true);
    }

    public void ShowLine(string speaker, string text)
    {
        Open();
        ClearChoices();
        StopTypingImmediate();

        //  대사 출력 시엔 대화창 다시 켜기
        ExitChoiceMode();

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
            dialogueText.color = Color.white;
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

    public bool IsTyping() => isTyping;

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

        //  선택지 뜰 때 대화창(프레임/배경) 숨김
        EnterChoiceMode();

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

    public void SetNextButtonActive(bool active)
    {
        if (nextButton != null)
            nextButton.gameObject.SetActive(active);
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

    public void ClearSlotSpeakers()
    {
        slotSpeakerMap.Clear();
    }

    public void ShowCutScene(int index)
    {
        Sprite sp = Resources.Load<Sprite>($"CutScene/{index}");
        if (sp == null)
        {
            Debug.LogWarning($"[CutScene] Sprite not found: CutScene/{index}");
            return;
        }

        cutSceneBackGround.SetActive(true);
        cutSceneRoot.SetActive(true);
        cutSceneRoot.transform.SetAsLastSibling();

        cutSceneImage.sprite = sp;
        cutSceneImage.gameObject.SetActive(true);

        Color c = cutSceneImage.color;
        c.a = 0f;
        cutSceneImage.color = c;

        if (cutSceneFadeCoroutine != null)
            StopCoroutine(cutSceneFadeCoroutine);

        cutSceneFadeCoroutine = StartCoroutine(FadeInCutScene());
    }

    private IEnumerator FadeInCutScene()
    {
        Color c = cutSceneImage.color;
        c.a = 0f;
        cutSceneImage.color = c;

        float t = 0f;
        const float duration = 0.6f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            c.a = Mathf.Clamp01(t / duration);
            cutSceneImage.color = c;
            yield return null;
        }

        c.a = 1f;
        cutSceneImage.color = c;
    }

    public void HideCutScene()
    {
        cutSceneImage.gameObject.SetActive(false);
        cutSceneRoot.SetActive(false);
        cutSceneBackGround.SetActive(false);
    }

    public void SetTransparency(float alpha)
    {
        panelCanvasGroup.alpha = alpha;
    }
}
