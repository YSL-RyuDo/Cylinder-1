using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CultistKillable : MonoBehaviour, IGriefKillable
{
    [Header("죽었을 때 바뀔 이미지(시체 스프라이트)")]
    public Sprite deadSprite;

    [Header("죽었을 때 비활성화할 컴포넌트들")]
    public MonoBehaviour[] disableBehaviours; // NPCDialogueTrigger 등
    public Collider2D[] disableColliders;     // 상호작용/충돌 콜라이더 등

    private bool killed = false;

    public void GriefKill()
    {
        if (killed) return;
        killed = true;

        Debug.Log($"[KILL] Cultist killed: {gameObject.name}");

        // 1) 스프라이트 교체
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null && deadSprite != null)
        {
            sr.sprite = deadSprite;
        }
        else
        {
            Debug.LogWarning("[KILL] deadSprite 또는 SpriteRenderer가 없음");
        }

        // 2) 상호작용/AI 같은 스크립트 꺼서 다시 말 못 걸게
        if (disableBehaviours != null)
        {
            foreach (var b in disableBehaviours)
                if (b != null) b.enabled = false;
        }

        // 3) 충돌/트리거 꺼서 막히거나 다시 트리거 안 되게
        if (disableColliders != null)
        {
            foreach (var c in disableColliders)
                if (c != null) c.enabled = false;
        }

        // (선택) 정렬/레이어 바꾸고 싶으면 여기서 sr.sortingOrder 조정 가능
    }
}
