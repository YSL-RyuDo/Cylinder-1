using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TalkManagerScript : MonoBehaviour
{
    public Text dialogueText;
    public GameObject nextText;
    public CanvasGroup dialogueGroup;
    public Queue<string> sentences;

    private string currentSentence;

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
    }

    public void Ondialogue(string[] lines)
    {
        sentences.Clear();
        foreach (string line in lines)
        {
            sentences.Enqueue(line);
        }
        dialogueGroup.alpha = 1;
        
        NextSentence();
    }

    public void NextSentence()
    {
        if(sentences.Count != 0)
        {
            currentSentence = sentences.Dequeue();
            istyping = true;
            nextText.SetActive(false);
            StartCoroutine(Typing(currentSentence));
        }
        else
        {
            dialogueGroup.alpha = 0;
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
        if(!istyping && Input.GetKeyDown(KeyCode.F))
            NextSentence();
        if (dialogueText.text.Equals(currentSentence)) 
        {
            nextText.SetActive(true);
            istyping = false;
        }
    }

}
