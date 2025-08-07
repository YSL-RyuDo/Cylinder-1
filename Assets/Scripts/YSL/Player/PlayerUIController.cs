using UnityEngine;
using DG.Tweening;
using System.Collections;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
    [Header("메뉴 패널 관련")]
    public GameObject menuPanel;
    public RectTransform menuBookCover;
    private CanvasGroup menuCanvasGroup;
    public Button closeButton;

    [Header("탭 관련 버튼들")]
    public RectTransform[] tabButtons;

    [Header("탭 버튼에 대응하는 패널들 (아이템, 맵, 세이브, 퀘스트 등)")]
    public GameObject[] tabPanels;

    [Header("선택선 이동 위치 (X 좌표)")]
    public float selectedOffsetX = -625f;

    [Header("북커버 애니메이션")]
    public float slideDuration = 0.5f;
    public float fadeDuration = 0.3f;
    public float selectorMoveDuration = 0.3f;

    private Vector2 hiddenPosition = new Vector2(0, -1000);
    private Vector2 visiblePosition = new Vector2(0, 0);

    private Vector2[] originalPositions;

    private bool isMenuOpen = false;
    private bool isAnimating = false;
    private int selectedButtonIndex = 0;

    public static PlayerUIController Instance { get; private set; }

    // 포탈 연동 관련
    private PortalTrigger currentPortal = null;
    private bool isPortalMode = false;

    // **아이템 사용 요청 관련 변수 추가**
    private bool hasRequestedUnlock = false;
    private int requestedItemId = -1;

    public bool IsPortalMode => isPortalMode;
    public PortalTrigger CurrentPortal => currentPortal;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        if (menuPanel != null)
        {
            menuPanel.SetActive(false);
            menuCanvasGroup = menuPanel.GetComponent<CanvasGroup>();
            if (menuCanvasGroup == null)
                menuCanvasGroup = menuPanel.AddComponent<CanvasGroup>();

            menuCanvasGroup.alpha = 0f;
            menuCanvasGroup.interactable = false;
            menuCanvasGroup.blocksRaycasts = false;
        }

        if (menuBookCover != null)
            menuBookCover.anchoredPosition = hiddenPosition;

        if (closeButton != null)
            closeButton.onClick.AddListener(OnCloseButtonClicked);

        if (tabButtons != null && tabButtons.Length > 0)
        {
            originalPositions = new Vector2[tabButtons.Length];
            for (int i = 0; i < tabButtons.Length; i++)
            {
                originalPositions[i] = tabButtons[i].anchoredPosition;

                int index = i;
                tabButtons[i].GetComponent<Button>().onClick.AddListener(() =>
                {
                    if (!isAnimating)
                    {
                        SelectButton(index);
                    }
                });
            }

            SelectButton(0);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (!isAnimating)
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

    public void CloseMenu()
    {
        if (!isAnimating && isMenuOpen)
        {
            isMenuOpen = false;
            StartCoroutine(CloseMenuRoutine());
        }
    }

    IEnumerator OpenMenuRoutine()
    {
        if (selectedButtonIndex == 0)
        {
            InventoryUIManager.Instance.Refresh();
        }

        isAnimating = true;

        menuPanel.SetActive(true);
        Canvas.ForceUpdateCanvases();

        menuBookCover.gameObject.SetActive(true);
        menuBookCover.DOKill();
        menuBookCover.anchoredPosition = hiddenPosition;

        menuCanvasGroup.alpha = 0f;
        menuCanvasGroup.interactable = true;
        menuCanvasGroup.blocksRaycasts = true;

        yield return null;

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
                    menuCanvasGroup.DOFade(0f, fadeDuration).SetEase(Ease.Linear);
                }
            });

        yield return moveTween.WaitForCompletion();

        while (menuCanvasGroup.alpha > 0.01f)
            yield return null;

        menuCanvasGroup.interactable = false;
        menuCanvasGroup.blocksRaycasts = false;
        menuPanel.SetActive(false);

        if (isPortalMode)
        {
            if (currentPortal != null)
            {
                if (!currentPortal.IsOpen)
                {
                    // 포탈 UI 닫힌 후 잠김 대사 출력
                    currentPortal.OnPortalUIClosedWithoutUse();
                }

                // 아이템 사용 요청이 있을 때만 잠금 해제 시도
                if (hasRequestedUnlock)
                {
                    currentPortal.TryUnlockPortalWithItem(requestedItemId);

                    hasRequestedUnlock = false;
                    requestedItemId = -1;
                }
            }

            isPortalMode = false;
            currentPortal = null;
        }

        isAnimating = false;
    }

    void OnCloseButtonClicked()
    {
        if (!isAnimating && isMenuOpen)
        {
            isMenuOpen = false;
            StartCoroutine(CloseMenuRoutine());
        }
    }

    void SelectButton(int index)
    {
        selectedButtonIndex = index;

        for (int i = 0; i < tabButtons.Length; i++)
        {
            Vector2 targetPos = originalPositions[i];

            if (i == selectedButtonIndex)
            {
                targetPos.x = selectedOffsetX;
            }

            tabButtons[i].DOKill();
            tabButtons[i].DOAnchorPos(targetPos, selectorMoveDuration).SetEase(Ease.OutCubic);
        }

        ShowOnlySelectedPanel(index);
    }

    void ShowOnlySelectedPanel(int index)
    {
        if (tabPanels == null || index < 0 || index >= tabPanels.Length)
            return;

        for (int i = 0; i < tabPanels.Length; i++)
        {
            tabPanels[i].SetActive(i == index);
        }

        if (index == 0)
        {
            InventoryUIManager.Instance.Refresh();
        }
    }

    public void OpenPortalMenu(PortalTrigger portal)
    {
        currentPortal = portal;
        isPortalMode = true;

        InventoryUIManager.Instance.SetPortalMode(true, portal);
        if (!isAnimating && !isMenuOpen)
        {
            selectedButtonIndex = 0; // 인벤토리 탭 강제 선택
            ToggleMenu();
        }
    }

    // 공개용: 아이템 사용 요청 기록 함수 (InventoryUIManager와 연결 가능)
    public void RequestPortalUnlock(int itemId)
    {
        hasRequestedUnlock = true;
        requestedItemId = itemId;
    }
}
