using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PersonalityUI : MonoBehaviour, IUIWindow
{
    public bool IsOpen { get; private set; }
    [SerializeField] private GameObject root;

    public Button chooseButton;

    public PersonalDialRotator dialRotator;
    public PlayerPersonality playerPersonality;

    private PersonalityDialogue currentDialogue;
    private int lastPreviewIndex = -1;

    [Header("선택 후 메인 버튼 UI")]
    public Image personalityButtonIcon;
    public Sprite[] personalityButtonSprites;

    private void Start()
    {
        root.SetActive(false);
        chooseButton.onClick.AddListener(OnChooseClicked);
    }


    public void Open()
    {
        IsOpen = true;
        root.SetActive(true);

        dialRotator.AlignToPersonality(playerPersonality.GetPersonalityIndex());

        UpdateUI();
        lastPreviewIndex = -1;
        HandlePreview();
    }


    public void Close()
    {
        dialRotator.isLocked = false;
        IsOpen = false;
        root.SetActive(false);

        FindAnyObjectByType<UIManager>().NotifyWindowClosed(this);
    }


    private void Update()
    {
        if (!IsOpen) return;

        UpdateChooseButton();
        UpdateUI();
        HandlePreview();
    }


    private void UpdateUI()
    {
        int index = dialRotator.CurrentIndex;
        currentDialogue = DialogueLoader.Instance.Get(index);
    }


    private void HandlePreview()
    {
        int index = dialRotator.CurrentIndex;
        if (index == lastPreviewIndex) return;

        lastPreviewIndex = index;
        DialogueManager.Instance.ShowDialogue(currentDialogue, true);
    }


    private void UpdateChooseButton()
    {
        int selected = dialRotator.CurrentIndex;
        int current = playerPersonality.GetPersonalityIndex();

        if (selected == 5)
        {
            chooseButton.interactable = false;
            return;
        }

        chooseButton.interactable = (selected != current);
    }


    private void OnChooseClicked()
    {
        dialRotator.isLocked = true;
        int selected = dialRotator.CurrentIndex;

        if (selected == 5) return;
        if (selected == playerPersonality.GetPersonalityIndex()) return;

        playerPersonality.SetPersonality((PersonalityType)selected);

        DialogueManager.Instance.ShowDialogue(currentDialogue, false);
        UpdatePersonalityButtonUI(selected);
        chooseButton.interactable = false;
        StartCoroutine(CloseAfterDelay(3f));
    }


    private IEnumerator CloseAfterDelay(float sec)
    {
        yield return new WaitForSecondsRealtime(sec);

        DialogueManager.Instance.HideDialogue();
        Close();
    }

    private void UpdatePersonalityButtonUI(int index)
    {
        // 아이콘 교체
        if (personalityButtonSprites != null && index < personalityButtonSprites.Length)
            personalityButtonIcon.sprite = personalityButtonSprites[index];
    }

}
