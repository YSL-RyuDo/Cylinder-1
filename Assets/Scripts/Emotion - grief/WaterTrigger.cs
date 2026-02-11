using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class WaterTrigger : MonoBehaviour
{
    [SerializeField] private float swimSpeed = 4f;

    private bool playerInside;
    private PlayerMove cachedMove;
    private GriefAbilityController cachedGrief;

    private bool IsGrief()
    {
        return PlayerPersonality.Instance != null &&
               PlayerPersonality.Instance.currentPersonality == PersonalityType.grief;
    }

    private void Apply(bool on)
    {
        if (cachedMove)
        {
            cachedMove.useSpeedOverride = on;
            if (on) cachedMove.speedOverride = swimSpeed;
        }

        if (cachedGrief)
        {
            if (on) cachedGrief.EnterWater();
            else cachedGrief.ExitWater();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = true;
        cachedMove = other.GetComponent<PlayerMove>();
        cachedGrief = other.GetComponent<GriefAbilityController>();

        // 들어올 때 비애면 즉시 적용
        Apply(IsGrief());
        Debug.Log("ENTER WATER TRIGGER");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = false;
        Apply(false);

        cachedMove = null;
        cachedGrief = null;
    }

    private void Update()
    {
        // 물 안에 있는 동안 인격이 바뀌면 즉시 반영
        if (!playerInside) return;
        Apply(IsGrief());
    }
}