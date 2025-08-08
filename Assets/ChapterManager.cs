using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapterManager : MonoBehaviour
{
    public static ChapterManager instance;
    public int currentChapter = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환에도 유지
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 현재 챕터 경로 (예: "Chapter01")
    public string GetChapterPath()
    {
        return $"Chapter{currentChapter:D2}";
    }

    // 챕터 변경
    public void SetChapter(int chapter)
    {
        currentChapter = chapter;
        Debug.Log($"현재 챕터 변경됨: {GetChapterPath()}");
    }

    // 현재 챕터 반환
    public int GetChapter()
    {
        return currentChapter;
    }
}