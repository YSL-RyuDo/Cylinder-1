using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerWaterController : MonoBehaviour
{
    [Tooltip("비애(grief) 상태에서 물에 있을 때 적용할 이동 속도")]
    [SerializeField] private float swimSpeed = 4f;

    private PlayerMove playerMove;
    private int waterCount;

    private void Awake()
    {
        playerMove = GetComponent<PlayerMove>();
    }

    public void EnterWater()
    {
        if (!IsGrief()) return;

        waterCount++;
        ApplySwimSpeed(true);
    }

    public void ExitWater()
    {
        waterCount = Mathf.Max(0, waterCount - 1);

        if (waterCount == 0)
            ApplySwimSpeed(false);
    }

    private bool IsGrief()
    {
        var pp = PlayerPersonality.Instance;
        return pp != null && pp.currentPersonality == PersonalityType.grief;
    }

    private void ApplySwimSpeed(bool on)
    {
        if (playerMove == null) return;

        playerMove.useSpeedOverride = on;
        if (on) playerMove.speedOverride = swimSpeed;
    }
}
