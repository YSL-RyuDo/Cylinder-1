using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

public class SealGroupUI : MonoBehaviour
{
    [Header("Puzzle Setup")]
    public List<Button> buttons;
    public Image markPrefab;

    private Image markInstance;
    private int answerIndex = -1;
    private Quaternion originalRotation;
    private LibraryPuzzlePanel panel;

    public bool IsAnswered { get; private set; } = false;
    public bool IsCorrect { get; private set; } = false;

    void Awake()
    {
        panel = FindObjectOfType<LibraryPuzzlePanel>();
        originalRotation = transform.rotation;
    }

    public void SetRandomAnswer()
    {
        answerIndex = Random.Range(0, buttons.Count);
        StartCoroutine(Glow(buttons[answerIndex]));
    }

    private IEnumerator Glow(Button btn)
    {
        var img = btn.GetComponent<Image>();
        Transform tr = btn.transform;

        img.color = Color.yellow;

        tr.localScale = Vector3.one;
        tr.DOScale(1.2f, 0.2f).SetLoops(2, LoopType.Yoyo);

        yield return new WaitForSeconds(1f);

        img.color = Color.white;
    }

    public void RotateGroup(float duration, bool clockwise, System.Action onComplete = null)
    {
        int spins = Random.Range(1, 2);
        float direction = clockwise ? -1f : 1f;
        float angle = spins * 360f * direction + 90f * direction;

        transform.DORotate(transform.eulerAngles + new Vector3(0, 0, angle), duration, RotateMode.FastBeyond360)
            .SetEase(Ease.InOutCubic)
            .OnComplete(() => onComplete?.Invoke());
    }


    public void EnableInput()
    {
        for (int i = 0; i < buttons.Count; i++)
            SetButtonListener(buttons[i], i);
    }

    private void SetButtonListener(Button button, int index)
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => OnClick(index));
    }

    private void OnClick(int index)
    {
        IsAnswered = true;
        IsCorrect = (index == answerIndex);

        ShowMarkOnButton(index);
        DisableButtons();

        panel.OnSealGroupAnswered();
    }

    private void ShowMarkOnButton(int index)
    {
        Button btn = buttons[index];
        RectTransform btnRect = btn.GetComponent<RectTransform>();

        if (markInstance != null)
            Destroy(markInstance.gameObject);

        Transform parent = panel.transform;  // 여기서 LibraryPuzzlePanel의 transform 사용
        markInstance = Instantiate(markPrefab, parent);

        var rt = markInstance.rectTransform;
        rt.position = btnRect.position;
        rt.rotation = Quaternion.identity;
        rt.localScale = Vector3.zero;

        markInstance.enabled = true;
        markInstance.raycastTarget = false;

        rt.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack);
    }


    private void DisableButtons()
    {
        foreach (var btn in buttons)
            btn.interactable = false;
    }

    public void ResetGroup()
    {
        IsAnswered = false;
        IsCorrect = false;
        transform.rotation = originalRotation;

        foreach (var btn in buttons)
        {
            btn.GetComponent<Image>().color = Color.white;
            btn.interactable = true;
        }

        if (markInstance != null)
        {
            Destroy(markInstance.gameObject);
            markInstance = null;
        }


        DOTween.Kill(transform);
    }
}
