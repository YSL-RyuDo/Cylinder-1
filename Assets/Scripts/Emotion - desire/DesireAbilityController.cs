using UnityEngine;
using System.Collections;

public class DesireAbilityController : MonoBehaviour
{
    [Header("UI Root")]
    [SerializeField] private GameObject desireUIRoot;

    [Header("Input")]
    [SerializeField] private KeyCode toggleKey = KeyCode.E;

    [Header("Animator")]
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private string castTriggerName = "Cast";

    [Header("옵션: 시전 중 입력 잠그기")]
    [SerializeField] private bool lockPlayerWhileCasting = true;
    [SerializeField] private float castLockTime = 0.6f; // 애니 길이에 맞춰

    [SerializeField] private PlayerMove playerMove; // 이동 스크립트(있으면 연결)

    private bool isOpen = false;
    private bool isCasting = false;

    private void Awake()
    {
        if (desireUIRoot != null)
            desireUIRoot.SetActive(false);

        if (playerAnimator == null)
            playerAnimator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (isCasting) return;

        if (Input.GetKeyDown(toggleKey))
        {
            // 열망 상태일 때만(너 enum이 desire 소문자라 했지)
            if (PlayerPersonality.Instance != null &&
                !PlayerPersonality.Instance.IsInState(PersonalityType.desire))
            {
                return;
            }

            StartCoroutine(CastAndToggleUI());
        }
    }

    private IEnumerator CastAndToggleUI()
    {
        isCasting = true;

        // 1) 시전 모션
        if (playerAnimator != null)
            playerAnimator.SetTrigger(castTriggerName);

        // 2) 이동 잠금(선택)
        if (lockPlayerWhileCasting && playerMove != null)
            playerMove.enabled = false;

        // 3) 모션 타임 기다리고 UI 토글
        yield return new WaitForSecondsRealtime(castLockTime);

        isOpen = !isOpen;
        if (desireUIRoot != null)
            desireUIRoot.SetActive(isOpen);

        // 4) 이동 해제
        if (lockPlayerWhileCasting && playerMove != null)
            playerMove.enabled = true;

        isCasting = false;
    }
}
