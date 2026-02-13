using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PuzzleLightingController : MonoBehaviour
{
    [Header("Global Light (어둡게/밝게)")]
    [SerializeField] private Light2D globalLight;

    [Header("Intensity")]
    [SerializeField] private float darkIntensity = 0.25f;
    [SerializeField] private float brightIntensity = 1.0f;

    private bool allCandlesLit;
    public bool IsAllCandlesLit => allCandlesLit;

    private void Awake()
    {
        ApplyState();
    }

    public void OnEnterPuzzle() => ApplyState();
    public void OnExitPuzzle() => ApplyState();

    public void MarkAllCandlesLit()
    {
        allCandlesLit = true;
        ApplyState();
    }

    private void ApplyState()
    {
        if (globalLight == null) return;
        globalLight.intensity = allCandlesLit ? brightIntensity : darkIntensity;
    }
}
