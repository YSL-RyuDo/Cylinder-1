using UnityEngine;

public class Candle : MonoBehaviour
{
    [Header("State")]
    [SerializeField] private bool isLit = false;
    public bool IsLit => isLit;

    [Header("Visual (placeholder)")]
    [SerializeField] private GameObject flameObject;
    [SerializeField] private bool allowToggleOff = false;
    [SerializeField] private string flameChildName = "Flame";

    private void Awake()
    {
        if (flameObject == null)
        {
            Transform t = transform.Find(flameChildName);
            if (t != null) flameObject = t.gameObject;
        }
    }

    private void Start()
    {
        isLit = false;
        RefreshVisual();
    }

    public void TryLight()
    {
        Debug.Log($"[Candle] Clicked: {name}");

        if (isLit && !allowToggleOff) return;

        isLit = allowToggleOff ? !isLit : true;
        RefreshVisual();
    }

    private void RefreshVisual()
    {
        if (flameObject != null)
            flameObject.SetActive(isLit);
    }
}


