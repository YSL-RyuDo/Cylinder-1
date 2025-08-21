using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TalkManagerScript : MonoBehaviour, IPointerDownHandler
{
    public Text tellerText;
    public Text dialogueText;
    public GameObject nextText;
    public GameObject LeftImg;
    public GameObject RightImg;
    public GameObject LogButton;
    public CanvasGroup dialogueGroup;
    public GameObject choicePanel;
    public GameObject choiceButtonPrefab;
    public GameObject fadeInObject;
    public GameObject fadeOutObject;
    public GameObject startObject;
    public Image dialogueBackgroundImage;
    public Sprite normalDialogueBG;
    public Sprite narrationBG;

    private bool waitingForSystemTrigger = false;
    private Dictionary<string, GameObject> systemMap;

    private Queue<DialogueNodeJSON.DialogueData> dialogues = new Queue<DialogueNodeJSON.DialogueData>();
    private DialogueNodeJSON.DialogueData currentDialogue;

    private string currentSentence;
    public float typingSpeed = 0.1f;
    private bool istyping;
    public static TalkManagerScript instance;

    // 로그
    public GameObject logPanel;
    public Text logText;
    private List<string> dialogueLog = new List<string>();

    private void Awake()
    {
        instance = this;
        systemMap = new Dictionary<string, GameObject>
        {
            { "FadeOut", fadeOutObject },
            { "FadeIn", fadeInObject },
            { "Start", startObject }
        };
    }

    DialogueNodeJSON LoadDialogueNode(string fileNameWithoutExtension)
    {
        // 현재 챕터 경로 붙이기
        string chapterPath = ChapterManager.instance.GetChapterPath();
        string fullPath = $"Dialogues/{chapterPath}/{fileNameWithoutExtension}";

        TextAsset ta = Resources.Load<TextAsset>(fullPath);
        if (ta == null)
        {
            Debug.LogError($"[TalkManager] Dialogue file not found: {fullPath}");
            return null;
        }
        return JsonUtility.FromJson<DialogueNodeJSON>(ta.text);
    }

    public void StartDialogue(string fileName)
    {
        DialogueNodeJSON node = LoadDialogueNode(fileName);
        InitializeDialogue(node.dialogues);
    }

    void InitializeDialogue(DialogueNodeJSON.DialogueData[] data)
    {
        dialogues.Clear();
        foreach (var d in data)
            dialogues.Enqueue(d);

        dialogueGroup.alpha = 1;
        dialogueGroup.blocksRaycasts = true;

        LeftImg.SetActive(true);
        RightImg.SetActive(true);
        LogButton.SetActive(true);

        NextSentence();
    }

    public void NextSentence()
    {
        if (waitingForSystemTrigger) return;

        if (dialogues.Count == 0)
        {
            dialogueGroup.alpha = 0;
            dialogueGroup.blocksRaycasts = false;
            LeftImg.SetActive(false);
            RightImg.SetActive(false);
            LogButton.SetActive(false);
            dialogueLog.Clear();
            return;
        }

        currentDialogue = dialogues.Dequeue();

        tellerText.text = currentDialogue.teller;
        currentSentence = currentDialogue.sentence;

        if (currentDialogue.isNarration)
        {
            LeftImg.SetActive(false);
            RightImg.SetActive(false);
            dialogueBackgroundImage.sprite = narrationBG;
            dialogueText.color = Color.white;
        }
        else
        {
            LeftImg.SetActive(true);
            RightImg.SetActive(true);
            dialogueBackgroundImage.sprite = normalDialogueBG;
            dialogueText.color = Color.black;
        }

        dialogueLog.Add(currentSentence);

        if (!string.IsNullOrEmpty(currentDialogue.leftSprite))
        {
            LeftImg.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Sprites/Standings/{currentDialogue.leftSprite}");
        }
        if (!string.IsNullOrEmpty(currentDialogue.rightSprite))
        {
            RightImg.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Sprites/Standings/{currentDialogue.rightSprite}");
        }

        istyping = true;
        nextText.SetActive(false);
        StartCoroutine(Typing(currentSentence));

        if (!string.IsNullOrEmpty(currentDialogue.systemName))
        {
            waitingForSystemTrigger = true;
        }

        if (currentDialogue.hasChoices)
        {
            StartCoroutine(ShowChoicesAfterTyping(currentDialogue.choices));
        }
    }

    IEnumerator ShowChoicesAfterTyping(DialogueNodeJSON.Choice[] choices)
    {
        while (istyping) yield return null;

        choicePanel.SetActive(true);
        foreach (Transform c in choicePanel.transform)
            Destroy(c.gameObject);

        foreach (var choice in choices)
        {
            GameObject btn = Instantiate(choiceButtonPrefab, choicePanel.transform);
            btn.GetComponentInChildren<Text>().text = choice.choiceText;

            string nextFile = choice.nextFile;
            btn.GetComponent<Button>().onClick.AddListener(() =>
            {
                choicePanel.SetActive(false);
                StartDialogue(nextFile);
            });
        }
    }

    IEnumerator Typing(string line)
    {
        dialogueText.text = "";
        foreach (char ch in line)
        {
            dialogueText.text += ch;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    void Update()
    {
        if (dialogueText.text == currentSentence)
        {
            nextText.SetActive(true);
            istyping = false;
        }
    }

    public void OnLogButton()
    {
        logPanel.SetActive(true);
        logText.text = "";
        foreach (var l in dialogueLog)
        {
            logText.text += l + "\n";
        }
    }

    public void OnCloseLog()
    {
        logPanel.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (istyping) return;

        if (waitingForSystemTrigger)
        {
            waitingForSystemTrigger = false;

            string[] sysNames = currentDialogue.systemName.Split(',');
            foreach (string raw in sysNames)
            {
                string name = raw.Trim();
                bool turnOff = false;
                if (name.EndsWith("_OFF"))
                {
                    turnOff = true;
                    name = name.Replace("_OFF", "");
                }

                if (systemMap.TryGetValue(name, out GameObject obj))
                {
                    obj.SetActive(!turnOff);
                }
            }

            dialogueGroup.alpha = 0;
            dialogueGroup.blocksRaycasts = false;
            LeftImg.SetActive(false);
            RightImg.SetActive(false);
            LogButton.SetActive(false);
            dialogueLog.Clear();
            return;
        }

        NextSentence();
    }
}
