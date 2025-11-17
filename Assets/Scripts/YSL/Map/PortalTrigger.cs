//using UnityEngine;
//using System.Collections;

//public class PortalTrigger : MonoBehaviour
//{
//    private PortalManager manager;
//    private DialogueLoader dialogueLoader;
//    private bool isOnCooldown = false;
//    private bool playerInRange = false;
//    private GameObject currentPlayer;

//    public int portalIndex;
//    private bool pendingUnlockAfterDialogue = false; // 대사 후 포탈 열기 여부

//    private void Start()
//    {
//        manager = FindObjectOfType<PortalManager>();
//        if (manager == null)
//            Debug.LogError("PortalManager를 찾을 수 없습니다.");

//        dialogueLoader = DialogueLoader.Instance;
//        if (dialogueLoader == null)
//            Debug.LogError("DialogueLoader가 준비되지 않았습니다.");

//        portalIndex = manager.GetPortalIndex(gameObject);
//    }

//    private void Update()
//    {
//        if (!playerInRange || currentPlayer == null || isOnCooldown) return;

//        bool isOpen = manager.IsPortalOpen(gameObject);

//        if (isOpen)
//        {
//            Transform dest = manager.GetDestination(gameObject);
//            if (dest != null)
//                StartCoroutine(TeleportWithFade(currentPlayer, dest));
//        }
//        else
//        {
//            if (Input.GetKeyDown(KeyCode.F))
//            {
//                PlayerUIController.Instance.OpenPortalMenu(this);
//            }
//        }
//    }

//    private void OnEnable()
//    {
//        if (DialogueManager.Instance != null)
//            DialogueManager.Instance.OnDialogueEnded += OnDialogueEnded;
//    }

//    private void OnDisable()
//    {
//        if (DialogueManager.Instance != null)
//            DialogueManager.Instance.OnDialogueEnded -= OnDialogueEnded;
//    }


//    public void TryUnlockPortalWithItem(int itemId)
//    {
//        if (portalIndex == 30 && itemId == 0 && InventoryManager.Instance.HasItem(0))
//        {
//            InventoryManager.Instance.RemoveItem(0);
//            pendingUnlockAfterDialogue = true;
//            ShowUnlockedDialogue(currentPlayer);
//        }
//    }

//    // 대사가 종료되면 호출되는 함수
//    private void OnDialogueEnded()
//    {
//        if (pendingUnlockAfterDialogue)
//        {
//            manager.SetPortalActive(portalIndex, true);
//            pendingUnlockAfterDialogue = false;
//        }
//    }

//    // 포탈 메뉴가 닫히고 포탈이 여전히 닫혀있다면 잠긴 대사 출력
//    public void OnPortalUIClosedWithoutUse()
//    {
//        if (!manager.IsPortalOpen(gameObject))
//        {
//            ShowLockedDialogue(currentPlayer);
//        }
//    }

//    private void OnTriggerEnter2D(Collider2D other)
//    {
//        if (!other.CompareTag("Player")) return;
//        playerInRange = true;
//        currentPlayer = other.gameObject;
//    }

//    private void OnTriggerExit2D(Collider2D other)
//    {
//        if (!other.CompareTag("Player")) return;
//        playerInRange = false;
//    }

//    // 잠긴 포탈 대사 출력
//    private void ShowLockedDialogue(GameObject player)
//    {
//        if (player == null)
//        {
//            Debug.LogWarning("ShowLockedDialogue 호출 시 player가 null입니다.");
//            return;
//        }

//        int personalityId = 0;

//        PlayerPersonality personality = player.GetComponent<PlayerPersonality>();
//        if (personality != null)
//            personalityId = personality.GetPersonalityIndex();

//        if (dialogueLoader == null)
//        {
//            Debug.LogWarning("DialogueLoader가 준비되지 않았습니다.");
//            return;
//        }

//        string[] lines = dialogueLoader.GetRandomLockedPortalDialogueLines(personalityId);
//        DialogueManager dialogueManager = DialogueManager.Instance;
//        if (dialogueManager != null)
//            dialogueManager.StartDialogue(lines, personalityId);
//        else
//            Debug.LogWarning("DialogueManager를 찾을 수 없습니다.");
//    }


//    // 열린 포탈 대사 출력
//    private void ShowUnlockedDialogue(GameObject player)
//    {
//        int personalityId = 0;

//        PlayerPersonality personality = player.GetComponent<PlayerPersonality>();
//        if (personality != null)
//            personalityId = personality.GetPersonalityIndex();

//        if (dialogueLoader == null)
//        {
//            Debug.LogWarning("DialogueLoader가 준비되지 않았습니다.");
//            return;
//        }

//        string[] lines = dialogueLoader.GetRandomUnlockedPortalDialogueLines(personalityId);
//        DialogueManager dialogueManager = DialogueManager.Instance;
//        if (dialogueManager != null)
//            dialogueManager.StartDialogue(lines, personalityId);
//        else
//            Debug.LogWarning("DialogueManager를 찾을 수 없습니다.");
//    }

//    private IEnumerator TeleportWithFade(GameObject player, Transform dest)
//    {
//        isOnCooldown = true;
//        PortalTrigger destTrigger = dest.GetComponent<PortalTrigger>();
//        if (destTrigger != null)
//            destTrigger.isOnCooldown = true;

//        // Fade out
//        yield return FadeManager.Instance.FadeOut();

//        PlayerMove playerMove = player.GetComponent<PlayerMove>();
//        if (playerMove != null)
//        {
//            MapBoundary parentMapBoundary = dest.GetComponentInParent<MapBoundary>();
//            if (parentMapBoundary != null)
//                playerMove.SetCurrentMapBoundary(parentMapBoundary);

//            Vector3 targetPos = dest.position;
//            RectTransform rectTransform = dest as RectTransform;
//            if (rectTransform != null)
//            {
//                Vector3 pivotLocalPos = new Vector3(
//                    rectTransform.rect.width * (rectTransform.pivot.x - 0.5f),
//                    rectTransform.rect.height * (rectTransform.pivot.y - 0.5f),
//                    0f
//                );
//                targetPos = rectTransform.TransformPoint(pivotLocalPos);
//            }

//            BoxCollider2D box = player.GetComponent<BoxCollider2D>();
//            if (box != null)
//            {
//                Vector3 colliderCenter = box.bounds.center;
//                Vector3 playerPivot = player.transform.position;

//                Vector3 offset = colliderCenter - playerPivot;
//                Vector3 finalPos = targetPos - offset;

//                playerMove.Warp(finalPos.x, finalPos.y);
//            }
//            else
//            {
//                playerMove.Warp(targetPos.x, targetPos.y);
//            }
//        }

//        // Fade in
//        yield return FadeManager.Instance.FadeIn();

//        yield return new WaitForSeconds(0.2f);
//        isOnCooldown = false;
//        if (destTrigger != null)
//            destTrigger.isOnCooldown = false;
//    }



//    public bool IsOpen
//    {
//        get
//        {
//            return manager != null && manager.IsPortalOpen(gameObject);
//        }
//    }
//}
