using UnityEngine;

public class LackObjectManager : MonoBehaviour
{
    [Header("Lack 상태일 때 활성화할 오브젝트")]
    public GameObject[] lackObjects;

    private void Start()
    {
        if (PlayerPersonality.Instance != null)
        {
            PlayerPersonality.Instance.OnPersonalityChanged += Apply;
            Apply(PlayerPersonality.Instance.currentPersonality);
        }
    }

    private void OnDestroy()
    {
        if (PlayerPersonality.Instance != null)
            PlayerPersonality.Instance.OnPersonalityChanged -= Apply;
    }

    private void Apply(PersonalityType type)
    {
        bool isLack = type == PersonalityType.lack;

        foreach (var obj in lackObjects)
        {
            if (obj != null)
                obj.SetActive(isLack);
        }
    }
}
