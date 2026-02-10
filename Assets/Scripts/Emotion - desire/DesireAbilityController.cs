using UnityEngine;

public class DesireAbilityController : MonoBehaviour
{
    [Header("UI Root (DesireUIRoot)")]
    [SerializeField] private GameObject desireUIRoot;

    [Header("토글 키")]
    [SerializeField] private KeyCode toggleKey = KeyCode.E;

    [Header("중복 입력 방지")]
    [SerializeField] private float toggleCooldown = 0.2f;

    private float lastToggleTime = -999f;
    private bool isOpen = false;

    private void Awake()
    {
        if (desireUIRoot != null)
            desireUIRoot.SetActive(false);
    }

    private void Update()
    {
        if (!Input.GetKeyDown(toggleKey))
            return;

        // 열망(Desire) 상태일 때만 토글 허용
        if (!IsDesireState())
        {
            Debug.Log("[Desire] 현재 인격이 Desire가 아니라서 UI 토글 막음");
            Close();
            return;
        }

        // 쿨다운
        if (Time.unscaledTime - lastToggleTime < toggleCooldown)
            return;

        lastToggleTime = Time.unscaledTime;

        if (isOpen) Close();
        else Open();
    }

    private bool IsDesireState()
    {
        if (PlayerPersonality.Instance == null)
        {
            Debug.LogWarning("[Desire] PlayerPersonality.Instance가 null임 (씬에 PlayerPersonality 오브젝트가 있는지 확인)");
            return false;
        }

        return PlayerPersonality.Instance.IsInState(PersonalityType.desire);
    }

    private void Open()
    {
        isOpen = true;
        if (desireUIRoot != null) desireUIRoot.SetActive(true);
        Debug.Log("[Desire] OPEN");
    }

    private void Close()
    {
        isOpen = false;
        if (desireUIRoot != null) desireUIRoot.SetActive(false);
        Debug.Log("[Desire] CLOSE");
    }
}
