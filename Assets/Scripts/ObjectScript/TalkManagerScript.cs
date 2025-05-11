using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TalkManagerScript : MonoBehaviour, IPointerDownHandler
{
    public Text dialogueText;
    public GameObject nextText;
    public GameObject LeftImg;
    public GameObject RightImg;
    public GameObject LogButton;
    public CanvasGroup dialogueGroup;
    public Queue<string> sentences;
    public Queue<Sprite> Lsprites;
    public Queue<Sprite> Rsprites;
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
    }
    void Start()
    {
        sentences = new Queue<string>();
        Lsprites = new Queue<Sprite>();
        Rsprites = new Queue<Sprite>();

    }

    public void Ondialogue(string[] lines, Sprite[] lsprite, Sprite[] rsprite)
    {
        sentences.Clear();
        Lsprites.Clear();
        Rsprites.Clear();

        foreach (string line in lines)
        {
            sentences.Enqueue(line);
        }

        foreach (Sprite sprite in lsprite)
        {
            Lsprites.Enqueue(sprite);
        }

        foreach (Sprite sprite in rsprite)
        {
            Rsprites.Enqueue(sprite);
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
        if(sentences.Count != 0)
        {
            currentSentence = sentences.Dequeue();
            currentLSprite = Lsprites.Dequeue();
            currentRSprite = Rsprites.Dequeue();
            dialogueLog.Add(currentSentence);

            // 오른쪽 이미지
            Image rightImage = RightImg.GetComponent<Image>();
            if (rightImage != null)
            {
                rightImage.sprite = currentRSprite;
            }

            // 왼쪽 이미지
            Image leftImage = LeftImg.GetComponent<Image>();
            if (leftImage != null)
            {
                leftImage.sprite = currentLSprite;
            }

            istyping = true;
            nextText.SetActive(false);
            StartCoroutine(Typing(currentSentence));
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
        Debug.Log("Log 버튼 눌림");
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
        if(!istyping)
            NextSentence();
    }
}
