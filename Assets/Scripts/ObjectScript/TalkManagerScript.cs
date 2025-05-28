using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static DialogueNode;

public class TalkManagerScript : MonoBehaviour, IPointerDownHandler
{
    public Text dialogueText;
    public GameObject nextText;
    public GameObject LeftImg;
    public GameObject RightImg;
    public GameObject LogButton;
    public CanvasGroup dialogueGroup;
    public GameObject choicePanel;
    public GameObject choiceButtonPrefab;
    public GameObject fadeOutObject;
    private Dictionary<string, GameObject> systemMap;

    private Queue<DialogueData> dialogues = new Queue<DialogueData>();
    private DialogueData currentDialogue;

    public GameObject logPanel;
    public Text logText;
    private List<string> dialogueLog = new List<string>();

    private string currentSentence;
    private Sprite currentLSprite;
    private Sprite currentRSprite;

    public float typingSpeed = 0.1f;
    private bool istyping;

    public static TalkManagerScript instance;
    private void Awake()
    {
        instance = this;
        systemMap = new Dictionary<string, GameObject>
    {
        { "FadeOut", fadeOutObject }
        // 필요한 시스템 오브젝트들을 여기에 추가
    };
    }

    public void Ondialogue(DialogueData[] data)
    {
        dialogues.Clear();
        foreach (DialogueData item in data)
        {
            dialogues.Enqueue(item);
        }

        dialogueGroup.alpha = 1;
        dialogueGroup.blocksRaycasts = true;

        LeftImg.SetActive(true);
        RightImg.SetActive(true);
        LogButton.SetActive(true);

        NextSentence();
    }

    public void NextSentence()
    {
        if (!string.IsNullOrEmpty(currentDialogue.systemName))
        {
            if (systemMap.TryGetValue(currentDialogue.systemName, out GameObject systemObj))
            {
                systemObj.SetActive(true);
                dialogueGroup.alpha = 0;
                dialogueGroup.blocksRaycasts = false;
                LeftImg.SetActive(false);
                RightImg.SetActive(false);
                LogButton.SetActive(false);
                dialogueLog.Clear();
            }
        }
        if (dialogues.Count != 0)
        {
            currentDialogue = dialogues.Dequeue();
            currentSentence = currentDialogue.sentence;
            currentLSprite = currentDialogue.leftSprite;
            currentRSprite = currentDialogue.rightSprite;

            dialogueLog.Add(currentSentence);

            LeftImg.GetComponent<Image>().sprite = currentLSprite;
            RightImg.GetComponent<Image>().sprite = currentRSprite;

            istyping = true;
            nextText.SetActive(false);
            StartCoroutine(Typing(currentSentence));

            if (currentDialogue.hasChoices)
            {
                StartCoroutine(ShowChoicesAfterTyping(currentDialogue.choices));
            }
        }
        else
        {
            dialogueGroup.alpha = 0;
            dialogueGroup.blocksRaycasts = false;
            LeftImg.SetActive(false);
            RightImg.SetActive(false);
            LogButton.SetActive(false);
            dialogueLog.Clear();
        }
    }

    IEnumerator ShowChoicesAfterTyping(DialogueNode.Choice[] choices)
    {
        while (istyping) yield return null;

        choicePanel.SetActive(true);
        foreach (Transform child in choicePanel.transform)
            Destroy(child.gameObject);

        foreach (var choice in choices)
        {
            GameObject btn = Instantiate(choiceButtonPrefab, choicePanel.transform);
            btn.GetComponentInChildren<Text>().text = choice.choiceText;

            DialogueNode nextNode = choice.nextNode;
            btn.GetComponent<Button>().onClick.AddListener(() =>
            {
                choicePanel.SetActive(false);
                Ondialogue(nextNode.dialogues);
            });
        }
    }

    IEnumerator Typing(string line)
    {
        dialogueText.text = "";
        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    void Update()
    {
        if (dialogueText.text.Equals(currentSentence))
        {
            nextText.SetActive(true);
            istyping = false;
        }
    }

    public void OnLogButton()
    {
        logPanel.SetActive(true);
        logText.text = "";
        foreach (string line in dialogueLog)
        {
            logText.text += line + "\n";
        }
    }

    public void OnCloseLog()
    {
        logPanel.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!istyping)
            NextSentence();
    }
}
