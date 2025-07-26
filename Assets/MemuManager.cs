using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject itemPanel;
    public GameObject savePanel;
    public GameObject mapPanel;
    public GameObject questPanel;

    public void OpenItemTab()
    {
        SetActiveTab(itemPanel);
    }

    public void OpenSaveTab()
    {
        SetActiveTab(savePanel);
    }

    public void OpenMapTab()
    {
        SetActiveTab(mapPanel);
    }

    public void OpenQuestTab()
    {
        SetActiveTab(questPanel);
    }

    private void SetActiveTab(GameObject selected)
    {
        itemPanel.SetActive(selected == itemPanel);
        savePanel.SetActive(selected == savePanel);
        mapPanel.SetActive(selected == mapPanel);
        questPanel.SetActive(selected == questPanel);
    }

}