using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class menuButtonHandler : MonoBehaviour
{
    public Text menuText;  // 메뉴 버튼 안의 텍스트 (예: "시작하기")

    public void OnMenuButtonClick()
    {
        string selected = menuText.text;

        switch (selected)
        {
            case "시작하기":
                Debug.Log("게임 시작!");
                SceneManager.LoadScene("InGame"); // 실제 씬 이름
                break;

            case "메뉴":
                Debug.Log("메뉴 창 열기");
                // 설정 UI 활성화 코드
                break;

            case "나가기":
                Debug.Log("게임 종료");
                Application.Quit();
                break;

            default:
                Debug.Log("알 수 없는 메뉴: " + selected);
                break;
        }
    }
}
