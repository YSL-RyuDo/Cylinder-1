using UnityEngine;

public class UIManager : MonoBehaviour
{
    public MenuUI menuUI;
    public PersonalityUI personalityUI;

    private IUIWindow currentWindow = null;

    void Update()
    {
        // UI가 열려 있을 때 ESC로 닫기
        if (currentWindow != null)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                currentWindow.Close();

            return; // UI가 열려 있을 때는 다른 입력 차단
        }

        // UI가 닫혀 있을 때만 단축키 허용
        if (Input.GetKeyDown(KeyCode.Q))
            OpenMenuDefault();

        if (Input.GetKeyDown(KeyCode.R))
            Open(personalityUI);
    }

    private void Open(IUIWindow window)
    {
        window.Open();
        currentWindow = window;

        Time.timeScale = 0f; // 게임 정지
    }

    public void NotifyWindowClosed(IUIWindow window)
    {
        if (currentWindow == window)
        {
            currentWindow = null;
            Time.timeScale = 1f; // 게임 재개
        }
    }

    // 메뉴 기본 모드 열기
    public void OpenMenuDefault()
    {
        if (currentWindow != null)
            return;

        menuUI.Open();
        currentWindow = menuUI;

        Time.timeScale = 0f;
    }

    // 메뉴 아이템 탭을 강제로 열기
    public void OpenMenuItemPanelFromButton()
    {
        if (currentWindow != null)
            return;

        menuUI.Open();
        menuUI.SelectTab(0); // 0번 = 아이템 탭
        currentWindow = menuUI;

        Time.timeScale = 0f;
    }

    // 버튼: 메뉴 기본 열기
    public void OpenMenuFromButton()
    {
        OpenMenuDefault();
    }

    // 버튼: 인격 UI 열기
    public void OpenPersonalityFromButton()
    {
        if (currentWindow == null)
            Open(personalityUI);
    }
}
