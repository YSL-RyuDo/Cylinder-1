using UnityEngine.SceneManagement;
using UnityEngine;

public class menuButtonHandler : MonoBehaviour
{
    public menudial menuDial; // menudial 스크립트 참조

    public void OnMenuButtonClick()
    {
        int selectedIndex = menuDial.SelectedIndex; // 선택된 인덱스 받기

        switch (selectedIndex)
        {
            case 0:
                Debug.Log("게임 시작!");
                SceneManager.LoadScene("InGame");
                break;
            case 1:
                Debug.Log("이어오기");
                // 이어오기 처리
                break;
            case 3:
                Debug.Log("메뉴 창 열기");
                // 설정 UI 활성화
                break;
            case 5:
                Debug.Log("게임 종료");
                Application.Quit();
                break;
            default:
                Debug.Log("알 수 없는 메뉴 선택: " + selectedIndex);
                break;
        }
    }
}
