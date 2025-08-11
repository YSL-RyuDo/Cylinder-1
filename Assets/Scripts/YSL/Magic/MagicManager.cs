using UnityEngine;
using UnityEngine.UI;

public class MagicManager : MonoBehaviour
{
    public static MagicManager Instance { get; private set; }

    [Header("매직 패널")]
    public GameObject magicPanel;

    [Header("인격 이미지")]
    public Image personalityImage; // 변경할 UI 이미지
    public Sprite[] personalitySprites; // 인격별 스프라이트 저장 (인덱스 = PersonalityType)

    private bool isPuzzleMode = false;
    private bool isMapMode = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleMagicPanel();
        }
    }

    public void SetPuzzleMode(bool enabled)
    {
        isPuzzleMode = enabled;
        if (enabled)
        {
            isMapMode = false;
            Debug.Log("MagicManager: 퍼즐 모드 활성화");
        }
        else
        {
            Debug.Log("MagicManager: 퍼즐 모드 비활성화");
        }
    }

    public void SetMapMode(bool enabled)
    {
        isMapMode = enabled;
        if (enabled)
        {
            isPuzzleMode = false;
            Debug.Log("MagicManager: 맵 모드 활성화");
        }
        else
        {
            Debug.Log("MagicManager: 맵 모드 비활성화");
        }
    }

    public bool IsPuzzleMode() => isPuzzleMode;
    public bool IsMapMode() => isMapMode;

    public void ToggleMagicPanel()
    {
        if (magicPanel == null)
        {
            Debug.LogWarning("MagicPanel이 할당되지 않았습니다.");
            return;
        }

        int personalityIndex = GetCurrentPersonalityIndex();

        // 인격 3, 4면 패널 열지 않음
        if (personalityIndex == 3 || personalityIndex == 4)
        {
            Debug.LogWarning("MagicManager: 인격 상태가 3 또는 4이므로 매직 패널을 열 수 없습니다.");
            return;
        }

        bool isActive = magicPanel.activeSelf;

        if (!isPuzzleMode && !isMapMode)
        {
            Debug.LogWarning("MagicManager: 퍼즐 모드 또는 맵 모드 중 하나를 활성화해야 매직 패널을 열 수 있습니다.");
            return;
        }

        magicPanel.SetActive(!isActive);
        Debug.Log($"MagicPanel {(isActive ? "닫힘" : "열림")}");

        // 패널 열릴 때 이미지 갱신
        if (!isActive)
        {
            UpdatePersonalityImage(personalityIndex);
        }
    }

    public void OpenMagicPanel()
    {
        if (magicPanel == null)
            return;

        int personalityIndex = GetCurrentPersonalityIndex();

        if (personalityIndex == 3 || personalityIndex == 4)
        {
            Debug.LogWarning("MagicManager: 인격 상태가 3 또는 4이므로 매직 패널을 열 수 없습니다.");
            return;
        }

        if (!magicPanel.activeSelf)
        {
            if (!isPuzzleMode && !isMapMode)
            {
                Debug.LogWarning("MagicManager: 모드가 활성화되지 않아 매직 패널을 열 수 없습니다.");
                return;
            }

            magicPanel.SetActive(true);

            // 패널 열릴 때 이미지 갱신
            UpdatePersonalityImage(personalityIndex);
        }
    }

    public void CloseMagicPanel()
    {
        if (magicPanel != null && magicPanel.activeSelf)
            magicPanel.SetActive(false);
    }

    public void UpdatePersonalityImage(int personalityIndex)
    {
        // 인격 3,4면 이미지 변경하지 않음
        if (personalityIndex == 3 || personalityIndex == 4)
            return;

        if (personalityImage != null && personalitySprites != null && personalityIndex < personalitySprites.Length)
        {
            personalityImage.sprite = personalitySprites[personalityIndex];
        }
    }

    private int GetCurrentPersonalityIndex()
    {
        if (PlayerPersonality.Instance != null)
            return PlayerPersonality.Instance.GetPersonalityIndex();
        else
            return -1;
    }
}
