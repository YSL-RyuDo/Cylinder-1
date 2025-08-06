using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class ExpressionSprite
{
    public string expression;
    public Sprite sprite;
}

[System.Serializable]
public class PersonalitySprite
{
    public int personalityId;
    public Sprite defaultSprite;
}

public class PortalDialogueUI : MonoBehaviour
{
    public static PortalDialogueUI Instance;

    [Header("UI Components")]
    public GameObject dialoguePanel;
    public Image playerImage;
    public TextMeshProUGUI dialogueText;

    [Header("표정별 스프라이트 매핑")]
    public List<ExpressionSprite> expressionSprites;

    [Header("인격별 기본 스프라이트")]
    public List<PersonalitySprite> personalityDefaultSprites;

    private Dictionary<string, Sprite> expressionSpriteDict;
    private Dictionary<int, Sprite> personalityDefaultSpriteDict;

    private Coroutine typingCoroutine;
    private bool isTyping = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            dialoguePanel.SetActive(false);
            InitExpressionDictionary();
            InitPersonalityDefaultSprites();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitExpressionDictionary()
    {
        expressionSpriteDict = new Dictionary<string, Sprite>();
        foreach (var entry in expressionSprites)
        {
            expressionSpriteDict[entry.expression] = entry.sprite;
        }
    }

    private void InitPersonalityDefaultSprites()
    {
        personalityDefaultSpriteDict = new Dictionary<int, Sprite>();
        foreach (var entry in personalityDefaultSprites)
        {
            personalityDefaultSpriteDict[entry.personalityId] = entry.defaultSprite;
        }
    }

    private string ExtractExpression(ref string text)
    {
        Regex regex = new Regex(@"\((.+?) 표정\)");
        var match = regex.Match(text);
        if (match.Success)
        {
            string expression = match.Groups[1].Value;
            text = regex.Replace(text, "").Trim();
            return expression;
        }
        return null;
    }

    private IEnumerator TypeText(string fullText)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in fullText)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(0.03f);
        }

        isTyping = false;
    }

    public bool OnClickShowTextOrNext(string fullText)
    {
        if (isTyping)
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }
            dialogueText.text = fullText;
            isTyping = false;
            return false;
        }
        else
        {
            return true;
        }
    }

    public void ShowDialogueSegment(string rawText, int personalityId)
    {
        string expression = ExtractExpression(ref rawText);

        if (!string.IsNullOrEmpty(expression))
        {
            if (expressionSpriteDict.TryGetValue(expression, out var newSprite))
            {
                playerImage.sprite = newSprite;
            }
            else
            {
                Debug.LogWarning($"[표정 없음] '{expression}'에 해당하는 스프라이트가 없습니다.");
                playerImage.sprite = GetDefaultSpriteForPersonality(personalityId);
            }
        }
        else
        {
            playerImage.sprite = GetDefaultSpriteForPersonality(personalityId);
        }

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeText(rawText));

        dialoguePanel.SetActive(true);
    }

    private Sprite GetDefaultSpriteForPersonality(int personalityId)
    {
        if (personalityDefaultSpriteDict != null && personalityDefaultSpriteDict.TryGetValue(personalityId, out var sprite))
        {
            return sprite;
        }
        else
        {
            Debug.LogWarning($"[기본 스프라이트 없음] personalityId {personalityId} 에 해당하는 기본 스프라이트가 없습니다.");
            return null;
        }
    }

    public void HideDialogue()
    {
        dialoguePanel.SetActive(false);
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
        isTyping = false;
    }
}
