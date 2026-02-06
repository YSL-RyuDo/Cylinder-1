using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableDisappear : MonoBehaviour, IGriefBreakable
{
    [Tooltip("부서질 때 같이 꺼야 하는 오브젝트(자식 포함) 있으면 넣기")]
    public GameObject rootToDisable;

    [Tooltip("부서질 때 이펙트(선택)")]
    public GameObject breakVfxPrefab;

    public void GriefBreak()
    {
        if (breakVfxPrefab)
            Instantiate(breakVfxPrefab, transform.position, Quaternion.identity);

        // 완전 제거
        if (rootToDisable != null) rootToDisable.SetActive(false);
        else gameObject.SetActive(false);

        // Destroy로 지우고 싶으면 위 대신 아래 사용
        // Destroy(rootToDisable != null ? rootToDisable : gameObject);
    }
}
