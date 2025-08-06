using UnityEngine;

public class PlayerPersonality : MonoBehaviour
{
    [Header("현재 인격 상태")]
    public PersonalityType currentPersonality = PersonalityType.pure;

    public void SetPersonality(PersonalityType newType)
    {
        currentPersonality = newType;

        Debug.Log("인격이 " + currentPersonality + " 상태로 변경됨");

    }

    public bool IsInState(PersonalityType type)
    {
        return currentPersonality == type;
    }

    public int GetPersonalityIndex()
    {
        return (int)currentPersonality;
    }
}
