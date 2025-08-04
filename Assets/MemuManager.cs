using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject menu;
    public GameObject itemPanel;
    public GameObject savePanel;
    public GameObject mapPanel;
    public GameObject questPanel;
    public GameObject[] turnOnObjects;
    public GameObject[] turnOffObjects;

    public MenuButton currentSelectedTab;

    public void OnTabSelected(MenuButton button)
    {
        currentSelectedTab = button;

        switch (button.tabType)
        {
            case MenuButton.TabType.Item:
                SetActiveTab(itemPanel);
                break;
            case MenuButton.TabType.Save:
                SetActiveTab(savePanel);
                break;
            case MenuButton.TabType.Map:
                SetActiveTab(mapPanel);
                break;
            case MenuButton.TabType.Quest:
                SetActiveTab(questPanel);
                break;
        }

        UpdateAllButtonVisuals();
    }

    private void OnEnable()
    {
        foreach (var item in turnOffObjects)
        {
            item.SetActive(false);
        }
        foreach (var item in turnOnObjects)
        {
            item.SetActive(true);
        }
    }

    private void SetActiveTab(GameObject selected)
    {
        itemPanel.SetActive(selected == itemPanel);
        savePanel.SetActive(selected == savePanel);
        mapPanel.SetActive(selected == mapPanel);
        questPanel.SetActive(selected == questPanel);
    }

    private void UpdateAllButtonVisuals()
    {
        foreach (var button in FindObjectsOfType<MenuButton>())
        {
            button.UpdateVisual();
        }
    }

    public void TurnOffMenu()
    {
        menu.SetActive(false);
        Time.timeScale = 1;
    }
}