using UnityEngine;

public class ObjectInteractionUI : MonoBehaviour
{
    public static ObjectInteractionUI Instance;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
