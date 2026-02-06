using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class WaterTileGateByPersonality : MonoBehaviour
{
    [Header("References")]
    [Tooltip("레이어를 바꿀 부모(Watertile) 오브젝트")]
    [SerializeField] private GameObject parentWaterTile;

    [Header("Layers")]
    [SerializeField] private string lockedLayerName = "Obstacle"; // 기본(막힘)
    [SerializeField] private string passLayerName = "Water";      // 비애일 때 통과

    private int lockedLayer;
    private int passLayer;

    private int griefInsideCount = 0;

    private void Awake()
    {
        lockedLayer = LayerMask.NameToLayer(lockedLayerName);
        passLayer = LayerMask.NameToLayer(passLayerName);

        if (lockedLayer == -1) Debug.LogError($"[{name}] Layer '{lockedLayerName}' 없음");
        if (passLayer == -1) Debug.LogError($"[{name}] Layer '{passLayerName}' 없음");

        if (parentWaterTile == null)
        {
            // 자식에 붙인 경우 기본은 부모를 자동 지정
            parentWaterTile = transform.parent ? transform.parent.gameObject : null;
        }
    }

    private void Reset()
    {
        // Trigger 강제
        var c = GetComponent<Collider2D>();
        if (c) c.isTrigger = true;

        if (parentWaterTile == null && transform.parent)
            parentWaterTile = transform.parent.gameObject;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (parentWaterTile == null) return;

        // 비애만 부모를 Water로 바꿈
        if (PlayerPersonality.Instance != null &&
            PlayerPersonality.Instance.currentPersonality == PersonalityType.grief)
        {
            griefInsideCount++;
            SetParentLayer(passLayer);
        }
        var pw = other.GetComponent<PlayerWaterController>();
        if (pw != null) pw.EnterWater();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (parentWaterTile == null) return;

        // grief로 들어온 경우만 카운트 감소
        griefInsideCount = Mathf.Max(0, griefInsideCount - 1);

        // 비애가 더 이상 안에 없으면 다시 잠금
        if (griefInsideCount == 0)
            SetParentLayer(lockedLayer);
        var pw = other.GetComponent<PlayerWaterController>();
        if (pw != null) pw.ExitWater();
    }

    private void Update()
    {
        // 비애로 들어가서 열어둔 상태인데, 인격이 바뀌면 즉시 다시 잠금
        if (griefInsideCount > 0)
        {
            bool isGrief = PlayerPersonality.Instance != null &&
                           PlayerPersonality.Instance.currentPersonality == PersonalityType.grief;
            if (!isGrief)
            {
                griefInsideCount = 0;
                SetParentLayer(lockedLayer);
            }
        }
    }

    private void SetParentLayer(int layer)
    {
        if (layer < 0) return;
        if (parentWaterTile.layer == layer) return;

        parentWaterTile.layer = layer;

        // 부모 아래 자식들도 같은 레이어로 맞추고 싶으면 아래 주석 해제
        // foreach (Transform t in parentWaterTile.transform)
        //     t.gameObject.layer = layer;
    }
}
