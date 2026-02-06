using UnityEngine;
using System.Collections.Generic;

public class PersonalityUIManager : MonoBehaviour
{
    public static PersonalityUIManager Instance;

    [Header("Personality UIs")]
    public List<PersonalityActionUI> personalityUIs;

    private PersonalityActionUI currentUI;

    private void Awake()
    {
        Instance = this;

        foreach (var ui in personalityUIs)
            ui.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleCurrentPersonalityUI();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseCurrentUI();
        }
    }

    void ToggleCurrentPersonalityUI()
    {
        PersonalityType currentPersonality =
            PlayerPersonality.Instance.currentPersonality;

        if (currentUI != null && currentUI.Personality == currentPersonality)
        {
            CloseCurrentUI();
            return;
        }

        CloseCurrentUI();

        foreach (var ui in personalityUIs)
        {
            if (ui.Personality == currentPersonality)
            {
                ui.Open();
                currentUI = ui;
                return;
            }
        }
    }

    void CloseCurrentUI()
    {
        if (currentUI != null)
        {
            currentUI.Close();
            currentUI = null;
        }
    }
}
