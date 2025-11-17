using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class PersonalDialRotator : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public RectTransform dialRoot;
    public float rotationSpeed = 0.2f;
    public float snapDuration = 0.25f;

    private Vector2 center;
    private Vector2 prevDir;

    private float currentZ = 0f;
    public int currentIndex = 0;

    public bool isLocked = false;   // 드래그 잠금 플래그 추가

    private float[] targetAngles = new float[]
    {
        0f, 60f, 120f, 180f, 240f, 300f
    };

    public int CurrentIndex => currentIndex;

    private void Start()
    {
        center = RectTransformUtility.WorldToScreenPoint(null, dialRoot.position);
    }

    public void AlignToPersonality(int index)
    {
        currentIndex = Mathf.Clamp(index, 0, targetAngles.Length - 1);
        float targetZ = -targetAngles[currentIndex];

        currentZ = targetZ;
        dialRoot.localRotation = Quaternion.Euler(0, 0, targetZ);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isLocked) return;   // 드래그 막기
        prevDir = eventData.position - center;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isLocked) return;   // 드래그 막기

        Vector2 dir = eventData.position - center;

        float delta = Vector2.SignedAngle(prevDir, dir);
        currentZ += delta;

        dialRoot.localRotation = Quaternion.Euler(0, 0, currentZ);
        prevDir = dir;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isLocked) return;   // 드래그 막기

        float normalized = NormalizeAngle(-currentZ);

        int nearest = 0;
        float minDist = Mathf.Abs(Mathf.DeltaAngle(normalized, targetAngles[0]));

        for (int i = 1; i < targetAngles.Length; i++)
        {
            float dist = Mathf.Abs(Mathf.DeltaAngle(normalized, targetAngles[i]));
            if (dist < minDist)
            {
                minDist = dist;
                nearest = i;
            }
        }

        currentIndex = nearest;
        float snapTargetZ = -targetAngles[nearest];

        StopAllCoroutines();
        StartCoroutine(SnapToAngle(snapTargetZ));
    }

    private IEnumerator SnapToAngle(float targetZ)
    {
        float elapsed = 0f;
        float startZ = currentZ;

        while (elapsed < snapDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            currentZ = Mathf.LerpAngle(startZ, targetZ, elapsed / snapDuration);
            dialRoot.localRotation = Quaternion.Euler(0, 0, currentZ);

            yield return null;
        }

        currentZ = targetZ;
        dialRoot.localRotation = Quaternion.Euler(0, 0, targetZ);
    }

    private float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle < 0f) angle += 360f;
        return angle;
    }
}
