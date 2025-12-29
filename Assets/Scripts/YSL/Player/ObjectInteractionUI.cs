using UnityEngine;

public class ObjectInteractionUI : MonoBehaviour
{
    public static ObjectInteractionUI Instance;

    public SpriteRenderer normalIcon;
    public SpriteRenderer lockedIcon;
    public SpriteRenderer talkIcon;   // Ãß°¡

    private void Awake()
    {
        Instance = this;

        Hide();
    }

    public void ShowNormal()
    {
        normalIcon.gameObject.SetActive(true);
        lockedIcon.gameObject.SetActive(false);
        talkIcon.gameObject.SetActive(false);
    }

    public void ShowLocked()
    {
        normalIcon.gameObject.SetActive(false);
        lockedIcon.gameObject.SetActive(true);
        talkIcon.gameObject.SetActive(false);
    }

    public void ShowTalk()
    {
        normalIcon.gameObject.SetActive(false);
        lockedIcon.gameObject.SetActive(false);
        talkIcon.gameObject.SetActive(true);
    }

    public void Hide()
    {
        normalIcon.gameObject.SetActive(false);
        lockedIcon.gameObject.SetActive(false);
        talkIcon.gameObject.SetActive(false);
    }
}
