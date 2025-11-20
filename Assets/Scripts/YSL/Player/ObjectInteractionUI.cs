using UnityEngine;

public class ObjectInteractionUI : MonoBehaviour
{
    public static ObjectInteractionUI Instance;

    public SpriteRenderer normalIcon;
    public SpriteRenderer lockedIcon;

    private void Awake()
    {
        Instance = this;

        normalIcon.gameObject.SetActive(false);
        lockedIcon.gameObject.SetActive(false);
    }

    public void ShowNormal()
    {
        normalIcon.gameObject.SetActive(true);
        lockedIcon.gameObject.SetActive(false);
    }

    public void ShowLocked()
    {
        normalIcon.gameObject.SetActive(false);
        lockedIcon.gameObject.SetActive(true);
    }

    public void Hide()
    {
        normalIcon.gameObject.SetActive(false);
        lockedIcon.gameObject.SetActive(false);
    }
}
