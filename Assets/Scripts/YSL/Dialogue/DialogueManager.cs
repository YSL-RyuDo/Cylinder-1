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
    [Header("인격전환 UI")]
    public TextMeshProUGUI dialogueText;
    public Image portraitImage;
    [Header("모노로그 UI")]
    public Image monologuePortraitImage;

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

        string[] folders = { "pure", "lack", "desire", "grief", "arrogance", "glitch" };

        foreach (string folder in folders)
        {
            Sprite[] sprites = Resources.LoadAll<Sprite>($"Characters/{folder}");

            foreach (var s in sprites)
            {
                string key = $"{folder}_{s.name}";
                emotionSprites[key] = s;

                Debug.Log($"[로드됨] {key}");
            }
        }
    }

    public void ShowLines(string[] lines)
    {
        if (lines == null || lines.Length == 0)
            return;

        string text = lines[Random.Range(0, lines.Length)];

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        dialogueText.text = "";
        dialogueText.gameObject.SetActive(true);

        typingCoroutine = StartCoroutine(TypeText(text));

        // 포탈 대사는 이미지 필요 없을 수도 있으므로 숨김
        portraitImage.gameObject.SetActive(false);
    }

    public void ShowPortalMonologue(DialogueBlock block, string folder)
    {
        if (block == null || block.lines == null || block.lines.Length == 0)
            return;

        string line = block.lines[Random.Range(0, block.lines.Length)];

        // 포탈 혼잣말 초상화 처리
        string emotionKey = $"{folder}_{block.emotion}";

        if (emotionSprites.TryGetValue(emotionKey, out Sprite sprite))
        {
            monologuePortraitImage.sprite = sprite;
            monologuePortraitImage.gameObject.SetActive(true);
        }
        else
        {
            monologuePortraitImage.gameObject.SetActive(false);
            Debug.LogWarning($"[PortalMono] 스프라이트 없음: {emotionKey}");
        }

        // MonologueUI 출력
        MonologueUI.Instance.Show(line);
    }

}
