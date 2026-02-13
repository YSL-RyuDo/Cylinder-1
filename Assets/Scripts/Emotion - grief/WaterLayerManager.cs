using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterLayerManager : MonoBehaviour
{
    [Header("Layer Names (정확히 레이어 이름과 동일해야 함)")]
    [SerializeField] private string waterLayerName = "Water";
    [SerializeField] private string obstacleLayerName = "Obstacle";

    [Header("Debug")]
    [SerializeField] private bool logEveryApply = true;

    private int waterLayer;
    private int obstacleLayer;

    private PersonalityType lastType;

    private void Awake()
    {
        waterLayer = LayerMask.NameToLayer(waterLayerName);
        obstacleLayer = LayerMask.NameToLayer(obstacleLayerName);

        if (waterLayer == -1)
        {
            Debug.LogError($"[WaterLayerManager] Layer '{waterLayerName}' 없음 (Project Settings > Tags and Layers 확인)");
            enabled = false;
            return;
        }
        if (obstacleLayer == -1)
        {
            Debug.LogError($"[WaterLayerManager] Layer '{obstacleLayerName}' 없음");
            enabled = false;
            return;
        }

        lastType = GetCurrentType();
        Apply("Awake");
    }

    private void Update()
    {
        var cur = GetCurrentType();
        if (cur == lastType) return;

        lastType = cur;
        Apply("Update");
    }

    public void ForceApply()
    {
        lastType = GetCurrentType();
        Apply("ForceApply");
    }

    private PersonalityType GetCurrentType()
    {
        return PlayerPersonality.Instance ? PlayerPersonality.Instance.currentPersonality : PersonalityType.pure;
    }

    private void Apply(string from)
    {
        bool isGrief = (GetCurrentType() == PersonalityType.grief);
        int layerToSet = isGrief ? waterLayer : obstacleLayer;

        //  매번 "WaterLayerTarget" 붙은 것만 찾아서 처리 (캐시/초기Water 조건 없음)
        var targets = FindObjectsOfType<WaterLayerTarget>(true);

        int changed = 0;
        foreach (var t in targets)
        {
            if (!t) continue;
            var go = t.Target;
            if (!go) continue;

            changed += SetLayerRecursively(go, layerToSet);
        }

        if (logEveryApply)
        {
            Debug.Log($"[WaterLayerManager:{from}] personality={GetCurrentType()} grief={isGrief} " +
                      $"targetsFound={targets.Length} changed={changed} -> {(isGrief ? waterLayerName : obstacleLayerName)}");
        }
    }

    private int SetLayerRecursively(GameObject go, int layer)
    {
        if (!go) return 0;

        //  WaterTrigger가 붙은 오브젝트(감지용)는 레이어 변경 금지
        if (go.GetComponent<WaterTrigger>() != null)
            return 0;

        int count = 0;
        if (go.layer != layer) { go.layer = layer; count++; }

        foreach (Transform c in go.transform)
            count += SetLayerRecursively(c.gameObject, layer);

        return count;
    }
}
