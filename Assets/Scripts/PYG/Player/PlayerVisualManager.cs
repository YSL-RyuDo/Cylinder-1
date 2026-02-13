using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PersonalityVisual
{
    public PersonalityType type;
    public AnimatorOverrideController overrideController;
}

public class PlayerVisualManager : MonoBehaviour
{
    [Header("컴포넌트 참조")]
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private BoxCollider2D col;

    [Header("인격별 외형 설정")]
    [SerializeField] private List<PersonalityVisual> visualSettings;

    [Header("수영 설정")]
    [SerializeField] private AnimatorOverrideController griefSwimController;
    [SerializeField] private LayerMask waterLayer;

    private const string TRANSFORM_TRIGGER = "DoTransform";
    private AnimatorOverrideController currentOverrideController;
    private PersonalityType lastCheckedPersonality;

    private bool wasInWater = false;

    private void Start()
    {
        if (PlayerPersonality.Instance != null)
        {
            lastCheckedPersonality = PlayerPersonality.Instance.currentPersonality;
            ApplyVisuals(lastCheckedPersonality);
        }
    }

    private void Update()
    {
        if (PlayerPersonality.Instance == null) return;

        PersonalityType currentType = PlayerPersonality.Instance.currentPersonality;

        if (PlayerPersonality.Instance != null)
        {
            if (PlayerPersonality.Instance.currentPersonality != lastCheckedPersonality)
            {
                PersonalityType newType = PlayerPersonality.Instance.currentPersonality;
                StartCoroutine(ChangePersonalityRoutine(newType));
                lastCheckedPersonality = newType;
            }
        }

        if (currentType == PersonalityType.grief)
        {
            HandleGriefWaterCheck();
        }
    }

    private void HandleGriefWaterCheck()
    {
        bool isInWater = IsTouchingWater();

        if (isInWater != wasInWater)
        {
            if (isInWater)
            {
                if (griefSwimController != null)
                    playerAnimator.runtimeAnimatorController = griefSwimController;
            }
            else
            {
                ApplyVisualsImmediate(PersonalityType.grief);
            }

            wasInWater = isInWater;
        }
    }

    private bool IsTouchingWater()
    {
        if (col == null) return false;

        Vector2 center = col.bounds.center;
        Vector2 size = col.bounds.size * 0.9f;

        return Physics2D.OverlapBox(center, size, 0f, waterLayer);
    }

    private void ApplyVisuals(PersonalityType type)
    {
        PersonalityVisual targetVisual = visualSettings.Find(x => x.type == type);

        if (targetVisual.overrideController != null)
        {
            playerAnimator.runtimeAnimatorController = targetVisual.overrideController;
        }
        else
        {
            Debug.LogWarning($"{type}에 대한 Animator Override Controller가 할당되지 않았습니다.");
        }
    }

    private IEnumerator ChangePersonalityRoutine(PersonalityType type)
    {
        Time.timeScale = 0f;

        AnimatorUpdateMode originalMode = playerAnimator.updateMode;
        playerAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;

        ApplyVisualsImmediate(type);

        playerAnimator.SetTrigger(TRANSFORM_TRIGGER);

        yield return new WaitForSecondsRealtime(0.1f);

        float animLength = playerAnimator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSecondsRealtime(animLength);

        playerAnimator.updateMode = originalMode;

        yield return new WaitForSecondsRealtime(0.5f);
        Time.timeScale = 1f;
    }
    private void ApplyVisualsImmediate(PersonalityType type)
    {
        PersonalityVisual targetVisual = visualSettings.Find(x => x.type == type);
        if (targetVisual.overrideController != null)
        {
            playerAnimator.runtimeAnimatorController = targetVisual.overrideController;
        }
    }
}