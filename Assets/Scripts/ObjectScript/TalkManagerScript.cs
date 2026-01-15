using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static DialogueNode;

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

    private Queue<DialogueData> dialogues = new Queue<DialogueData>();
    private DialogueData currentDialogue;

    public GameObject logPanel;
    public Text logText;
    private List<string> dialogueLog = new List<string>();

    private string currentName;
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
            { "FadeOut", fadeOutObject },
            { "FadeIn", fadeInObject },
            { "Start", startObject }
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

        currentName = currentDialogue.teller;
        tellerText.text = "";
        tellerText.text = currentName;
        currentSentence = currentDialogue.sentence;
        currentLSprite = currentDialogue.leftSprite;
        currentRSprite = currentDialogue.rightSprite;
        
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

        LeftImg.GetComponent<Image>().sprite = currentLSprite;
        RightImg.GetComponent<Image>().sprite = currentRSprite;

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

    IEnumerator ShowChoicesAfterTyping(DialogueNode.Choice[] choices)
    {
        while (istyping) yield return null;

        choicePanel.SetActive(true);
<<<<<<< Updated upstream
<<<<<<< Updated upstream
        foreach (Transform child in choicePanel.transform)
            Destroy(child.gameObject);
=======
=======
>>>>>>> Stashed changes

        foreach (Transform c in choicePanel.transform)
            Destroy(c.gameObject);
>>>>>>> Stashed changes

        foreach (var choice in choices)
        {
            GameObject btn = Instantiate(choiceButtonPrefab, choicePanel.transform);
<<<<<<< Updated upstream

            
            btn.GetComponentInChildren<Text>().text = choice.choiceText;
            


<<<<<<< Updated upstream
            DialogueNode nextNode = choice.nextNode;
            btn.GetComponent<Button>().onClick.AddListener(() =>
            {
                choicePanel.SetActive(false);
                Ondialogue(nextNode.dialogues);
=======
            string nextFile = choice.nextFile;

            btn.GetComponent<Button>().onClick.AddListener(() =>
            {
                choicePanel.SetActive(false);
=======

            
            btn.GetComponentInChildren<Text>().text = choice.choiceText;
            


            string nextFile = choice.nextFile;

            btn.GetComponent<Button>().onClick.AddListener(() =>
            {
                choicePanel.SetActive(false);
>>>>>>> Stashed changes

              
                if (string.IsNullOrEmpty(nextFile) || nextFile == "END")
                {
                    dialogueGroup.alpha = 0;
                    dialogueGroup.blocksRaycasts = false;
                    LeftImg.SetActive(false);
                    RightImg.SetActive(false);
                    LogButton.SetActive(false);
                    dialogueLog.Clear();
                    return;
                }

               
                StartDialogue(nextFile);
>>>>>>> Stashed changes
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
        if (istyping) return;

        if (waitingForSystemTrigger)
        {
            waitingForSystemTrigger = false;

            string[] sysNames = currentDialogue.systemName.Split(',');

            foreach (string rawName in sysNames)
            {
                string sysName = rawName.Trim();
                bool turnOff = false;

                if (sysName.EndsWith("_OFF"))
                {
                    turnOff = true;
                    sysName = sysName.Replace("_OFF", "");
                }

                if (systemMap.TryGetValue(sysName, out GameObject systemObj))
                {
                    systemObj.SetActive(!turnOff);
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
