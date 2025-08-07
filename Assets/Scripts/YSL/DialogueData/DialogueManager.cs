using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    private string[] currentSegments;
    private int currentIndex = 0;
    private bool isDialogueActive = false;
    private int currentPersonalityId = 0;
    public static DialogueManager Instance { get; private set; }
    public event System.Action OnDialogueEnded;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    public void StartDialogue(string[] segments, int personalityId)
    {
        currentSegments = segments;
        currentIndex = 0;
        isDialogueActive = true;
        currentPersonalityId = personalityId;
        ShowCurrentSegment();
    }

    private void ShowCurrentSegment()
    {
        if (currentIndex < currentSegments.Length)
        {
            PersonalDialogueUI.Instance.ShowDialogueSegment(currentSegments[currentIndex], currentPersonalityId);
        }
        else
        {
            EndDialogue();
        }
    }

    public void NextSegment()
    {
        if (!isDialogueActive) return;

        currentIndex++;
        if (currentIndex < currentSegments.Length)
        {
            ShowCurrentSegment();
        }
        else
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        isDialogueActive = false;
        PersonalDialogueUI.Instance.HideDialogue();

        OnDialogueEnded?.Invoke();  // 대사 종료 이벤트 호출
    }

    private void Update()
    {
        if (isDialogueActive && Input.GetMouseButtonDown(0))
        {
            string currentText = currentSegments[currentIndex];

            bool canGoNext = PersonalDialogueUI.Instance.OnClickShowTextOrNext(currentText);
            if (canGoNext)
            {
                NextSegment();
            }
        }
    }
}
