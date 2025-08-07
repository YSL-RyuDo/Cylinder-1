// PersonaDialRotator.cs
using UnityEngine;
using DG.Tweening;

public class PersonaDialRotator : MonoBehaviour
{
    public Transform dialRoot;
    public float rotationSpeed = 0.2f;
    public float snapDuration = 0.3f;
    public float spinDuration = 1.0f;

    public PlayerPersonality playerPersonality;

    private bool isDragging = false;
    private Vector3 lastMousePos;
    private int currentIndex = 0;

    private float[] targetAngles = new float[] { 0f, -300f, -240f, -180f, -120f, -60f };

    public int CurrentIndex => currentIndex;

    void Start()
    {
        int startIndex = 0;

        if (playerPersonality != null)
        {
            startIndex = playerPersonality.GetPersonalityIndex();
            if (startIndex < 0 || startIndex >= targetAngles.Length)
                startIndex = 0;
        }

        currentIndex = startIndex;
        dialRoot.rotation = Quaternion.Euler(0, 0, targetAngles[startIndex]);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            lastMousePos = Input.mousePosition;
            dialRoot.DOKill();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            SnapToNearestAngle();
        }

        if (isDragging && dialRoot != null)
        {
            Vector3 delta = Input.mousePosition - lastMousePos;
            float rotationZ = -delta.x * rotationSpeed;

            dialRoot.Rotate(Vector3.forward, rotationZ);
            lastMousePos = Input.mousePosition;
        }
    }

    public void SnapToNearestAngle()
    {
        float zRotation = NormalizeAngle(dialRoot.eulerAngles.z);

        float nearestAngle = targetAngles[0];
        float minDist = Mathf.Abs(Mathf.DeltaAngle(zRotation, targetAngles[0]));
        int nearestIndex = 0;

        for (int i = 1; i < targetAngles.Length; i++)
        {
            float dist = Mathf.Abs(Mathf.DeltaAngle(zRotation, targetAngles[i]));
            if (dist < minDist)
            {
                minDist = dist;
                nearestAngle = targetAngles[i];
                nearestIndex = i;
            }
        }

        currentIndex = nearestIndex;

        dialRoot.DOKill();
        dialRoot
            .DORotate(new Vector3(0, 0, nearestAngle), snapDuration)
            .SetEase(Ease.OutCubic);
    }

    float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle > 180f) angle -= 360f;
        return angle;
    }

    public void PlayIntroAnimation()
    {
        dialRoot.DOKill();

        dialRoot
            .DORotate(new Vector3(0, 0, dialRoot.eulerAngles.z - 360f * 3), spinDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                SnapToNearestAngle();
            });
    }
}
