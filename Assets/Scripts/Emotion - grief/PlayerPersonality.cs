using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPersonality : MonoBehaviour
{
    public static PlayerPersonality Instance { get; private set; }

    [Header("현재 인격 상태")]
    public PersonalityType currentPersonality = PersonalityType.pure;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

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
