using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialRotator : MonoBehaviour
{
    [System.Serializable]
    public class SlotData
    {
        public RectTransform slotButton; // 슬롯의 중심
        public float maxAngle;           // 해당 슬롯이 도는 최대 각도 (예: 60, 90, ...)
    }

    public RectTransform dialTransform;
    public string DialogueNode;
    public List<SlotData> slots;
    public PlayerScript playerScript;
    public GameObject FadeIn;

    private bool isDragging = false;
    private Vector2 centerScreen;
    private float currentAngle = 0f;
    private float dragStartAngle = 0f;
    private float dragMaxAngle = 0f;
    private int selectedSlot = -1;

    public float returnSpeed = 360f;

    void Start()
    {
        centerScreen = RectTransformUtility.WorldToScreenPoint(Camera.main, dialTransform.position);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            int hit = GetSlotIndexUnderMouse();
            if (hit != -1)
            {
                selectedSlot = hit;
                dragMaxAngle = slots[hit].maxAngle;
                dragStartAngle = GetAngleFromCenter(Input.mousePosition);
                isDragging = true;
            }
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            float currentMouseAngle = GetAngleFromCenter(Input.mousePosition);
            float delta = Mathf.DeltaAngle(dragStartAngle, currentMouseAngle);

            if (delta < 0)
            {
                float targetAngle = Mathf.Clamp(-delta, 0f, dragMaxAngle);
                currentAngle = targetAngle;
                dialTransform.localRotation = Quaternion.Euler(0f, 0f, -currentAngle);
            }
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            StartCoroutine(ReturnDial());

            if (playerScript != null && selectedSlot >= 0)
            {
                playerScript.ChangeIdentity((PlayerScript.identityType)selectedSlot);
            }
        }
    }

    IEnumerator ReturnDial()
    {
        while (Mathf.Abs(currentAngle) > 0.1f)
        {
            currentAngle = Mathf.Lerp(currentAngle, 0f, Time.deltaTime * returnSpeed);
            dialTransform.localRotation = Quaternion.Euler(0f, 0f, -currentAngle);
            yield return null;
        }

        currentAngle = 0f;
        dialTransform.localRotation = Quaternion.identity;

        yield return new WaitForSeconds(0.3f);
        transform.parent.gameObject.SetActive(false);
        FadeIn.SetActive(true);
        TalkManagerScript.instance.StartDialogue(DialogueNode);
        PlayerScript player = FindObjectOfType<PlayerScript>();
        player.Warp(0f, 0f);
    }

    float GetAngleFromCenter(Vector2 screenPos)
    {
        Vector2 dir = screenPos - centerScreen;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        return (angle + 360f) % 360f;
    }

    int GetSlotIndexUnderMouse()
    {
        Vector2 mouse = Input.mousePosition;
        for (int i = 0; i < slots.Count; i++)
        {
            Vector2 slotScreenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, slots[i].slotButton.position);
            if (Vector2.Distance(mouse, slotScreenPos) < 50f) // 클릭 범위
            {
                return i;
            }
        }
        return -1;
    }
}
