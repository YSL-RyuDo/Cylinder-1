using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PersonalityUI : MonoBehaviour, IUIWindow
{
    public bool IsOpen { get; private set; }
    [SerializeField] private GameObject root;

    public Button chooseButton;
    public TextMeshProUGUI nameText;
    public PersonalDialRotator dialRotator;
    public PlayerPersonality playerPersonality;

    private PersonalityDialogue currentDialogue;
    private int lastPreviewIndex = -1;

    [Header("선택 후 메인 버튼 UI")]
    public Image personalityButtonIcon;
    public Sprite[] personalityButtonSprites;

    private void Awake()
    {
        root.SetActive(false);
        IsOpen = false;
    }

    private void Start()
    {
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
        currentDialogue = DialogueLoader.Instance.PersonalityGet(index);

        // 이름 갱신
        if (currentDialogue != null && nameText != null)
            nameText.text = currentDialogue.name;
    }


    private void HandlePreview()
    {
        int index = dialRotator.CurrentIndex;
        if (index == lastPreviewIndex) return;

        lastPreviewIndex = index;
        //DialogueManager.Instance.ShowDialogue(currentDialogue, true);
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

        //인격변경시 애니메이션 출력 기능 때문에 주석처리
        //playerPersonality.SetPersonality((PersonalityType)selected);

        //DialogueManager.Instance.ShowDialogue(currentDialogue, false);
        UpdatePersonalityButtonUI(selected);
        chooseButton.interactable = false;
        //코루틴에서 이제 인격 변경
        StartCoroutine(CloseAfterDelay(3f, (PersonalityType)selected));
    }


    private IEnumerator CloseAfterDelay(float sec, PersonalityType newType)
    {
        yield return new WaitForSecondsRealtime(sec);

        //DialogueManager.Instance.HideDialogue();
        Close();
        // 인격 변경 추가
        playerPersonality.SetPersonality(newType);
    }

    private void UpdatePersonalityButtonUI(int index)
    {
        // 아이콘 교체
        if (personalityButtonSprites != null && index < personalityButtonSprites.Length)
            personalityButtonIcon.sprite = personalityButtonSprites[index];
    }

}
