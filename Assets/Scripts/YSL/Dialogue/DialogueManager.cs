using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

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
            Debug.LogWarning($"스프라이트 없음: {emotionKey}");
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

            if (sprites == null || sprites.Length == 0)
            {
                Debug.LogWarning($"폴더가 비어있거나 없음: Characters/{folder}");
                continue;
            }

            foreach (Sprite s in sprites)
            {
                string key = $"{folder}_{s.name}";
                emotionSprites[key] = s;

                Debug.Log($"로드됨: {key}");
            }
        }

        Debug.Log($"감정 스프라이트 로드 완료 → 총 {emotionSprites.Count}개");
    }

}
