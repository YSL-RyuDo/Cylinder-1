using UnityEngine;
using TMPro;
using System.Collections;

public class NarrationUI : MonoBehaviour
{
    public static NarrationUI Instance;

    public GameObject root; // ÆÐ³Î
    public TextMeshProUGUI text;

    [SerializeField] private float typingSpeed = 0.03f;

    private string fullText;
    private Coroutine typingCo;
    private bool isTyping = false;
    private bool isOpen = false;

    private void Awake()
    {
        Instance = this;
        root.SetActive(false);
    }

    public void ShowMessage(string message)
    {
        if (typingCo != null)
            StopCoroutine(typingCo);

        fullText = message;
        text.text = "";
        isTyping = true;
        isOpen = true;

        root.SetActive(true);

        typingCo = StartCoroutine(TypeRoutine());
    }

    private IEnumerator TypeRoutine()
    {
        foreach (char c in fullText)
        {
            text.text += c;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }

        isTyping = false;
    }

    private void Update()
    {
        if (!isOpen) return;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                if (typingCo != null)
                    StopCoroutine(typingCo);

                text.text = fullText;
                isTyping = false;
            }
            else
            {
                Hide();
            }
        }
    }

    public void Hide()
    {
        root.SetActive(false);
        isOpen = false;
    }
}
