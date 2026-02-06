using UnityEngine;

public abstract class PersonalityActionUI : MonoBehaviour, IUIWindow
{
    public abstract PersonalityType Personality { get; }

    public bool IsOpen => gameObject.activeSelf;

    public virtual void Open()
    {
        gameObject.SetActive(true);
    }

    public virtual void Close()
    {
        gameObject.SetActive(false);
        UIManager.Instance.NotifyWindowClosed(this);
    }
}
