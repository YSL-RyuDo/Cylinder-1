using UnityEngine;
using DG.Tweening;
using System.Collections;

public class PlayerUIController : MonoBehaviour
{
    public GameObject menuPanel;
    public RectTransform menuBookCover;
    private CanvasGroup menuCanvasGroup;

    public float slideDuration = 0.5f;
    public float fadeDuration = 0.3f;

    private Vector2 hiddenPosition = new Vector2(0, -1000);
    private Vector2 visiblePosition = new Vector2(0, 0);

    private bool isMenuOpen = false;
    private bool isAnimating = false; // 애니메이션 진행 중 체크 변수

    void Start()
    {
        if (menuPanel != null)
        {
            menuPanel.SetActive(false);
            menuCanvasGroup = menuPanel.GetComponent<CanvasGroup>();
            if (menuCanvasGroup == null)
                menuCanvasGroup = menuPanel.AddComponent<CanvasGroup>();

            menuCanvasGroup.alpha = 0f;  // 처음에 완전 투명
            menuCanvasGroup.interactable = false;
            menuCanvasGroup.blocksRaycasts = false;
        }

        if (menuBookCover != null)
            menuBookCover.anchoredPosition = hiddenPosition;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (!isAnimating)  // 애니메이션 중이 아니면 토글 가능
            {
                ToggleMenu();
            }
        }
    }

    void ToggleMenu()
    {
        isMenuOpen = !isMenuOpen;

        if (isMenuOpen)
        {
            StartCoroutine(OpenMenuRoutine());
        }
        else
        {
            StartCoroutine(CloseMenuRoutine());
        }
    }

    IEnumerator OpenMenuRoutine()
    {
        isAnimating = true;

        menuPanel.SetActive(true);
        Canvas.ForceUpdateCanvases();

        menuBookCover.gameObject.SetActive(true);
        menuBookCover.DOKill();
        menuBookCover.anchoredPosition = hiddenPosition;

        // CanvasGroup 초기 상태 세팅
        menuCanvasGroup.alpha = 0f;
        menuCanvasGroup.interactable = true;
        menuCanvasGroup.blocksRaycasts = true;

        yield return null; // 한 프레임 대기

        // 위치 애니메이션 + 페이드인 동시 실행
        Tween moveTween = menuBookCover.DOAnchorPos(visiblePosition, slideDuration).SetEase(Ease.OutCubic);
        Tween fadeTween = menuCanvasGroup.DOFade(1f, fadeDuration).SetEase(Ease.Linear);

        yield return DOTween.Sequence()
            .Join(moveTween)
            .Join(fadeTween)
            .WaitForCompletion();

        isAnimating = false;
    }

    IEnumerator CloseMenuRoutine()
    {
        isAnimating = true;

        menuBookCover.DOKill();

        bool fadeStarted = false;

        Tween moveTween = menuBookCover.DOAnchorPos(hiddenPosition, slideDuration).SetEase(Ease.InCubic)
            .OnUpdate(() =>
            {
                if (!fadeStarted && menuBookCover.anchoredPosition.y <= -500f)
                {
                    fadeStarted = true;
                    // 페이드 아웃 시작
                    menuCanvasGroup.DOFade(0f, fadeDuration).SetEase(Ease.Linear);
                }
            });

        yield return moveTween.WaitForCompletion();

        // 페이드 아웃이 끝나지 않았을 수도 있으니 확실히 기다리기
        while (menuCanvasGroup.alpha > 0.01f)
            yield return null;

        menuCanvasGroup.interactable = false;
        menuCanvasGroup.blocksRaycasts = false;
        menuPanel.SetActive(false);

        isAnimating = false;
    }

}
