using UnityEngine;

public class DesireAbilityController : MonoBehaviour
{
    [Header("UI Root")]
    [SerializeField] private GameObject desireUIRoot;

    [Header("Input")]
    [SerializeField] private KeyCode toggleKey = KeyCode.E;

    private bool isOpen = false;

    private void Awake()
    {
        if (desireUIRoot != null)
            desireUIRoot.SetActive(false);
    }

    private void Update()
    {
        if (!Input.GetKeyDown(toggleKey)) return;

        // 열망 상태일 때만
        if (PlayerPersonality.Instance != null &&
            !PlayerPersonality.Instance.IsInState(PersonalityType.desire))
            return;

        // 퍼즐(벽 상호작용) 중이면 E로 마법진 안 뜨게
        if (CandlePuzzleController.Instance != null &&
            CandlePuzzleController.Instance.State == CandlePuzzleController.GameState.PuzzleView)
            return;

        isOpen = !isOpen;

        if (desireUIRoot != null)
            desireUIRoot.SetActive(isOpen);
    }

    public void ForceCloseUI()
    {
        isOpen = false;
        if (desireUIRoot != null) desireUIRoot.SetActive(false);
    }
}
