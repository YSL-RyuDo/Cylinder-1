using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class menudial : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public RectTransform dialImage;
    public Text selectionText;
    public List<string> menuItems;

    private Vector2 centerScreenPos;
    private Vector2 previousDirection;
    private float currentZRotation = 0f;

    private List<float> menuAngles = new List<float>();
    private const float fullCircle = 360f;
    public int SelectedIndex { get; private set; } = 0;

    void Start()
    {
        InitializeDial();
        UpdateSelection();
    }

    private void InitializeDial()
    {
        if (dialImage != null)
        {
            dialImage.pivot = new Vector2(0.5f, 0.5f);
            dialImage.anchorMin = dialImage.anchorMax = new Vector2(0.5f, 0.5f);
        }

        float segment = fullCircle / menuItems.Count;
        menuAngles.Clear();

        for (int i = 0; i < menuItems.Count; i++)
        {
            float angle = (i * segment + fullCircle) % fullCircle;
            menuAngles.Add(angle);
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

    public void OnEndDrag(PointerEventData eventData)
    {
        float targetAngle = GetClosestMenuAngle(GetNormalizedAngle(currentZRotation));
        float targetZRotation = fullCircle - targetAngle;

        StartCoroutine(SmoothSnapTo(targetZRotation));
    }

    private float GetNormalizedAngle(float angle)
    {
        return (fullCircle - (angle % fullCircle) + fullCircle) % fullCircle;
    }

    private float GetClosestMenuAngle(float angle)
    {
        float closest = menuAngles[0];
        float minDelta = Mathf.Abs(Mathf.DeltaAngle(angle, closest));

        foreach (float menuAngle in menuAngles)
        {
            float delta = Mathf.Abs(Mathf.DeltaAngle(angle, menuAngle));
            if (delta < minDelta)
            {
                minDelta = delta;
                closest = menuAngle;
            }
        }

        return closest;
    }

    private IEnumerator SmoothSnapTo(float targetZ)
    {
        float duration = 0.2f;
        float elapsed = 0f;
        float startZ = currentZRotation;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            currentZRotation = Mathf.LerpAngle(startZ, targetZ, elapsed / duration);
            dialImage.localRotation = Quaternion.Euler(0, 0, currentZRotation);
            yield return null;
        }

        currentZRotation = targetZ;
        dialImage.localRotation = Quaternion.Euler(0, 0, currentZRotation);

        UpdateSelection();
    }

    

    private void UpdateSelection()
    {
        float angle = GetNormalizedAngle(currentZRotation);
        int count = menuItems.Count;
        int rawIndex = Mathf.RoundToInt(angle / (fullCircle / count)) % count;
        int index = (count - rawIndex) % count;

        SelectedIndex = index;
        selectionText.text = menuItems[index];
    }

}
