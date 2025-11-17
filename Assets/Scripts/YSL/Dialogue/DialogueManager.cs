using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [SerializeField] private float typingSpeed = 0.03f;

    public TextMeshProUGUI dialogueText;
    public Image portraitImage;

    private Coroutine typingCoroutine;

    public Dictionary<string, Sprite> emotionSprites = new Dictionary<string, Sprite>();

    private void Awake()
    {
        Instance = this;
        LoadEmotionSprites();
    }

    public void ShowDialogue(PersonalityDialogue data, bool isPreview)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        DialogueBlock block = isPreview ? data.preview : data.confirm;
        if (block == null) return;

        string text = block.lines[Random.Range(0, block.lines.Length)];

        string emotionKey = $"{data.folder}_{block.emotion}";

        if (emotionSprites.TryGetValue(emotionKey, out Sprite sprite))
        {
            portraitImage.sprite = sprite;
            portraitImage.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning($"스프라이트를 찾을 수 없음: {emotionKey}");
            portraitImage.gameObject.SetActive(false);
        }

        dialogueText.text = "";
        dialogueText.gameObject.SetActive(true);

        typingCoroutine = StartCoroutine(TypeText(text));
    }


    private IEnumerator TypeText(string fullText)
    {
        dialogueText.text = "";

        foreach (char c in fullText)
        {
            dialogueText.text += c;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }
    }

    public void HideDialogue()
    {
        dialogueText.text = "";
        dialogueText.gameObject.SetActive(false);
        portraitImage.gameObject.SetActive(false);
    }

    private void LoadEmotionSprites()
    {
        emotionSprites = new Dictionary<string, Sprite>();

        // Characters 폴더의 모든 하위 폴더 자동 탐색
        TextAsset dummy = Resources.Load<TextAsset>("dummy"); // 강제 Resources 초기화 (일부 환경에서 필요)

        // Resources.LoadAll은 폴더 직접 검색 불가 → 대신 Sprite 전체 로드 후 폴더명 분석
        Sprite[] allSprites = Resources.LoadAll<Sprite>("Characters");

        if (allSprites.Length == 0)
        {
            Debug.LogError("Characters 폴더에서 스프라이트를 찾을 수 없습니다.");
            return;
        }

        foreach (var s in allSprites)
        {
            // 예: Characters/pure/0 → "pure"
            string fullPath = s.texture.name;
            string folderName = GetFolderName(s);

            if (folderName == null)
            {
                Debug.LogWarning($"폴더명을 추출할 수 없음: {s.name}");
                continue;
            }

            string key = $"{folderName}_{s.name}";
            emotionSprites[key] = s;

            Debug.Log($"[로드됨] {key}");
        }

        Debug.Log($"[완료] 감정 스프라이트 총 {emotionSprites.Count}개 로드");
    }


    // 텍스처 경로에서 폴더명 추출
    private string GetFolderName(Sprite s)
    {
        string path = AssetDatabase.GetAssetPath(s);
        // 예: Assets/Resources/Characters/pure/0.png
        // → pure 추출
        int index = path.IndexOf("Characters/");
        if (index == -1) return null;

        string sub = path.Substring(index + "Characters/".Length);
        int slash = sub.IndexOf('/');
        if (slash == -1) return null;

        return sub.Substring(0, slash);
    }

}
