using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialRotator : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public PlayerScript playerScript;
    private RectTransform rectTransform;
    private Vector2 centerScreenPos;
    private float currentAngle = 0f;
    public GameObject Identity;
    public DialogueNode dialogueNode;
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        centerScreenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, rectTransform.position);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)){
            Identity.SetActive(false);
            if (TalkManagerScript.instance.dialogueGroup.alpha == 0)
            {
                TalkManagerScript.instance.Ondialogue(dialogueNode.dialogues);
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        centerScreenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, rectTransform.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPosFrom;
        Vector2 localPosTo;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform, eventData.pressPosition, eventData.pressEventCamera, out localPosFrom);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform, eventData.position, eventData.pressEventCamera, out localPosTo);

        float angle = Vector2.SignedAngle(localPosFrom, localPosTo);
        currentAngle += angle;
        rectTransform.rotation = Quaternion.Euler(0, 0, currentAngle);

        // 다음 각도 기준을 위해 초기화
        eventData.pressPosition = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float snapped = Mathf.Round(currentAngle / 60f) * 60f;
        currentAngle = snapped;
        rectTransform.rotation = Quaternion.Euler(0, 0, currentAngle);

        int selectedIndex = Mathf.RoundToInt((-currentAngle % 360) / 60f);
        if (selectedIndex < 0) selectedIndex += 6;
        Debug.Log("선택된 슬롯: " + selectedIndex);
        if (playerScript != null)
        {
            PlayerScript.identityType newIdentity = (PlayerScript.identityType)selectedIndex;
            playerScript.ChangeIdentity(newIdentity);
        }
    }
}
