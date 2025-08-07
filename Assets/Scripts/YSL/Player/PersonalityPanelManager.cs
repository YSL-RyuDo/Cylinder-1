// PersonalityPanelManager.cs
using UnityEngine;
using UnityEngine.UI;

public class PersonalityPanelManager : MonoBehaviour
{
    [Header("인격 패널")]
    public GameObject personalityPanel;  // 인격 패널 전체

    [Header("UI 요소")]
    public Button chooseButton;           // 추즈 버튼
    public Button closeButton;            // 닫기 버튼

    public PersonaDialRotator dialRotator; // 회전 스크립트 참조
    public PlayerPersonality playerPersonality; // 플레이어 인격 스크립트 참조

    void Start()
    {
        if (personalityPanel != null)
            personalityPanel.SetActive(false);

        if (chooseButton != null)
            chooseButton.onClick.AddListener(OnChooseButtonClicked);

        if (closeButton != null)
            closeButton.onClick.AddListener(() =>
            {
                personalityPanel.SetActive(false);
            });
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (personalityPanel != null)
                personalityPanel.SetActive(!personalityPanel.activeSelf);

            if (personalityPanel.activeSelf && dialRotator != null && playerPersonality != null)
            {
                dialRotator.PlayIntroAnimation();
                UpdateChooseButtonInteractable();
            }
        }

        // 실시간으로 버튼 상태 갱신 (원하면 Update에서 삭제 가능)
        if (personalityPanel != null && personalityPanel.activeSelf)
        {
            UpdateChooseButtonInteractable();
        }
    }

    void UpdateChooseButtonInteractable()
    {
        if (chooseButton == null || dialRotator == null || playerPersonality == null)
            return;

        int selectedIndex = dialRotator.CurrentIndex;
        int currentPersonalityIndex = playerPersonality.GetPersonalityIndex();

        bool canChoose = selectedIndex != currentPersonalityIndex;

        chooseButton.interactable = canChoose;
    }


    void OnChooseButtonClicked()
    {
        if (playerPersonality == null || dialRotator == null)
        {
            Debug.LogWarning("PlayerPersonality 또는 PersonaDialRotator가 할당되지 않았습니다!");
            return;
        }

        int selectedIndex = dialRotator.CurrentIndex;

        // 인격 전환 불가 (예외 인격 5번)
        if (selectedIndex == 5)
        {
            DialogueManager dialogueManager = DialogueManager.Instance;
            if (dialogueManager != null && DialogueLoader.Instance != null)
            {
                Debug.Log("실행중이다");
                Debug.Log(playerPersonality.GetPersonalityIndex());
                string[] failLines = DialogueLoader.Instance
                    .GetRandomPersonalitySwitchFailDialogueLines(playerPersonality.GetPersonalityIndex());
                Debug.Log(failLines);
                dialogueManager.StartDialogue(failLines, playerPersonality.GetPersonalityIndex());
            }
            else
            {
                Debug.LogWarning("DialogueManager 또는 DialogueLoader를 찾을 수 없습니다.");
            }

            return;
        }

        // 동일한 인격 선택 방지
        if (selectedIndex == playerPersonality.GetPersonalityIndex())
        {
            Debug.Log("현재 인격은 선택할 수 없습니다.");
            return;
        }

        // 인격 전환
        playerPersonality.SetPersonality((PersonalityType)selectedIndex);
        Debug.Log($"인격 {(PersonalityType)selectedIndex} 가 적용되었습니다.");

        UpdateChooseButtonInteractable(); // 선택 후 버튼 상태 갱신

        if (personalityPanel != null)
        {
            personalityPanel.SetActive(false); // 선택 후 패널 닫기
        }

        DialogueManager dialogueManagerSuccess = DialogueManager.Instance;
        if (dialogueManagerSuccess != null && DialogueLoader.Instance != null)
        {
            // 전환 후 인격의 성공 대사 가져오기 (전환된 인격의 selectedIndex 사용)
            string[] successLines = DialogueLoader.Instance.GetRandomPersonalitySwitchSuccessDialogueLines(selectedIndex);
            dialogueManagerSuccess.StartDialogue(successLines, selectedIndex);
        }
        else
        {
            Debug.LogWarning("DialogueManager 또는 DialogueLoader를 찾을 수 없습니다.");
        }
    }

}
