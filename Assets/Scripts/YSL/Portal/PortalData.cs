using UnityEngine;

[CreateAssetMenu(menuName = "Game/Portal")]
public class PortalData : ScriptableObject
{
    public int portalId;
    public int linkedPortalId;

    [Header("초기 잠김 여부")]
    public bool initiallyLocked = false;

    [HideInInspector]
    public bool isLocked = false;

    public int requiredItemId = -1;
    public int requiredPuzzleId = -1;

    public Vector2 destinationOffset;

    public void ResetRuntimeState()
    {
        isLocked = initiallyLocked;
    }
}
