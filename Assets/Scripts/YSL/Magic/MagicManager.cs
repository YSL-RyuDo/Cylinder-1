using UnityEngine;

public class MagicManager : MonoBehaviour
{
    public static MagicManager Instance { get; private set; }

    [Header("매직 패널")]
    public GameObject magicPanel;

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
            isMapMode = false; // 퍼즐 모드 활성화 시 맵 모드는 끔
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
            isPuzzleMode = false; // 맵 모드 활성화 시 퍼즐 모드는 끔
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

        bool isActive = magicPanel.activeSelf;

        // 모드가 둘 다 비활성화된 경우 패널 열지 않음
        if (!isPuzzleMode && !isMapMode)
        {
            Debug.LogWarning("MagicManager: 퍼즐 모드 또는 맵 모드 중 하나를 활성화해야 매직 패널을 열 수 있습니다.");
            return;
        }

        magicPanel.SetActive(!isActive);
        Debug.Log($"MagicPanel {(isActive ? "닫힘" : "열림")}");
    }

    public void OpenMagicPanel()
    {
        if (magicPanel != null && !magicPanel.activeSelf)
        {
            if (!isPuzzleMode && !isMapMode)
            {
                Debug.LogWarning("MagicManager: 모드가 활성화되지 않아 매직 패널을 열 수 없습니다.");
                return;
            }
            magicPanel.SetActive(true);
        }
    }

    public void CloseMagicPanel()
    {
        if (magicPanel != null && magicPanel.activeSelf)
            magicPanel.SetActive(false);
    }
}
