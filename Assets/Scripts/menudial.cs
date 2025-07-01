using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class menudial : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    public RectTransform dialImage;     // 회전 대상
    public Text selectionText;          // 선택된 항목 표시
    public List<string> menuItems;      // 메뉴 항목 리스트

    private Vector2 centerScreenPos;    // 회전 중심
    private Vector2 previousDirection;  // 이전 방향
    private float currentZRotation = 0; // 회전 누적값

    void Start()
    {
        if (dialImage != null)
        {
            dialImage.pivot = new Vector2(0.5f, 0.5f);
            dialImage.anchorMin = dialImage.anchorMax = new Vector2(0.5f, 0.5f);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        centerScreenPos = RectTransformUtility.WorldToScreenPoint(null, dialImage.position);
        previousDirection = eventData.position - centerScreenPos;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 currentDirection = eventData.position - centerScreenPos;

        float angleDelta = Vector2.SignedAngle(previousDirection, currentDirection);

        currentZRotation += angleDelta;
        dialImage.localRotation = Quaternion.Euler(0, 0, currentZRotation);

        previousDirection = currentDirection;

        UpdateSelection();
    }

    private void UpdateSelection()
    {
        float normalizedAngle = (360f - (currentZRotation % 360f) + 360f) % 360f;

        float segment = 360f / menuItems.Count;

        for (int i = 0; i < menuItems.Count; i++)
        {
            float centerAngle = i * segment;
            float minAngle = (centerAngle - 30f + 360f) % 360f;
            float maxAngle = (centerAngle + 30f) % 360f;

            if (minAngle < maxAngle)
            {
                if (normalizedAngle >= minAngle && normalizedAngle < maxAngle)
                {
                    selectionText.text = menuItems[i];
                    return;
                }
            }
            else
            {
                if (normalizedAngle >= minAngle || normalizedAngle < maxAngle)
                {
                    selectionText.text = menuItems[i];
                    return;
                }
            }
        }

        selectionText.text = "";
    }
}
