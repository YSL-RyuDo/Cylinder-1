using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class MobAwarenessVisualToggle : MonoBehaviour
{
    [SerializeField] private GameObject visualRoot;

    private void OnEnable()
    {
        Apply();
    }

    private void Update()
    {
        // 인격이 바뀔 수 있으니 간단히 매 프레임 반영(가볍고 안전)
        Apply();
    }

    private void Apply()
    {
        bool show = PlayerPersonality.Instance != null &&
                    PlayerPersonality.Instance.currentPersonality == PersonalityType.grief;

        if (visualRoot && visualRoot.activeSelf != show)
            visualRoot.SetActive(show);
    }
}
