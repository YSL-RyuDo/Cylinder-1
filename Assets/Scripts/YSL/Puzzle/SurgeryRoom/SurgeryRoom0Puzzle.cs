//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class SurgeryRoom0Puzzle : PuzzleTrigger
//{
//    [Header("SurgeryRoom0 퍼즐 전용")]
//    public GameObject surgeryRoomPanel;  // 퍼즐 패널 (필요시 사용)
//    public Image puzzleImage;             // 퍼즐 이미지
//    public Sprite[] puzzleSprites;       // 상태별 이미지 스프라이트 배열

//    [Header("아이템 관련")]
//    [SerializeField]
//    private int[] requiredItems = new int[] { 1, 2, 3, 4, 5 };

//    private HashSet<int> usedItems = new HashSet<int>();

//    [Header("퍼즐 클리어시 비활성화할 크리쳐")]
//    public GameObject creatureObject;

//    [Header("연동 퍼즐 참조")]
//    public SurgeryRoom1Puzzle linkedPuzzle1;

//    protected override void Start()
//    {
//        base.Start();

//        UpdatePuzzleImage();
//    }

//    protected override void Update()
//    {
//        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F))
//        {
//            int playerPersonality = GetPlayerPersonality();

//            if (playerPersonality == -1)
//            {
//                Debug.LogWarning("플레이어 인격 정보를 얻을 수 없어 퍼즐에 진입할 수 없습니다.");
//                return;
//            }

//            if (playerPersonality == 4)
//            {
//                string[] enterDialogue = DialogueLoader.Instance.GetSurgeryRoomEnterDialogue(playerPersonality);
//                DialogueManager.Instance.StartDialogue(enterDialogue, playerPersonality);

//                Debug.Log("인격 4: 퍼즐 열지 않고 대사만 출력");
//                return;
//            }

//            if (PuzzleManager.Instance.CanPlayerEnterPuzzle(puzzleID, playerPersonality))
//            {
//                PuzzleManager.Instance.OpenPuzzle(puzzleID, this);
//                Debug.Log($"퍼즐 열기 시도: {puzzleID}");

//                if (InventoryUIManager.Instance != null)
//                    InventoryUIManager.Instance.SetPuzzleMode(true, this);

//                if (MagicManager.Instance != null)
//                {
//                    MagicManager.Instance.SetPuzzleMode(true);
//                }

//                // 대사 호출 추가 부분
//                string[] enterDialogue = DialogueLoader.Instance.GetSurgeryRoomEnterDialogue(playerPersonality);
//                DialogueManager.Instance.StartDialogue(enterDialogue, playerPersonality);
//            }
//            else
//            {
//                Debug.Log($"인격 {playerPersonality}은(는) 퍼즐 {puzzleID}에 진입할 수 없습니다.");
//            }
//        }
//    }

//    public override void TryUseItemOnPuzzle(int itemId)
//    {
//        if (!isPlayerInRange)
//        {
//            Debug.LogWarning("퍼즐 범위 밖입니다.");
//            return;
//        }

//        if (currentPlayer == null)
//        {
//            Debug.LogWarning("currentPlayer가 null입니다.");
//            return;
//        }

//        bool hasItem = InventoryManager.Instance.HasItem(itemId);

//        if (!System.Array.Exists(requiredItems, x => x == itemId))
//        {
//            Debug.LogWarning($"이 퍼즐에서 필요한 아이템이 아닙니다: {itemId}");
//            return;
//        }

//        if (!hasItem)
//        {
//            Debug.LogWarning($"아이템 {itemId}이(가) 인벤토리에 없습니다.");
//            return;
//        }

//        if (usedItems.Contains(itemId))
//        {
//            Debug.LogWarning($"이미 아이템 {itemId}을(를) 사용했습니다.");
//            return;
//        }

//        // 아이템 사용 처리
//        InventoryManager.Instance.RemoveItem(itemId);
//        usedItems.Add(itemId);
//        Debug.Log($"아이템 {itemId} 사용 완료. 남은 아이템: {requiredItems.Length - usedItems.Count}");

//        // 이미지 갱신
//        UpdatePuzzleImage();

//        // 모두 사용했으면 클리어 처리
//        if (usedItems.Count == requiredItems.Length)
//        {
//            PuzzleManager.Instance.MarkCleared(puzzleID);
//            gameObject.SetActive(false);
//            Debug.Log("퍼즐 클리어 완료, 퍼즐 오브젝트 비활성화");

//            if (creatureObject != null)
//            {
//                creatureObject.SetActive(false);
//            }

//            if (linkedPuzzle1 != null && !PuzzleManager.Instance.IsCleared(linkedPuzzle1.puzzleID))
//            {
//                linkedPuzzle1.ForceClearPuzzle();
//            }

//            PortalManager portalManager = FindObjectOfType<PortalManager>();
//            if (portalManager != null)
//            {
//                int portalIndex = 3;
//                portalManager.SetPortalActive(portalIndex, true);
//                Debug.Log("Portal3의 AtoB와 BtoA를 활성화했습니다.");
//            }
//            else
//            {
//                Debug.LogWarning("PortalManager를 찾을 수 없습니다.");
//            }

//            // 퍼즐 모드 끄고 매직 패널 닫기
//            if (MagicManager.Instance != null)
//            {
//                MagicManager.Instance.SetPuzzleMode(false);
//                //MagicManager.Instance.CloseMagicPanel();
//            }
//        }

//    }

//    private void UpdatePuzzleImage()
//    {
//        if (puzzleImage == null || puzzleSprites == null || puzzleSprites.Length == 0)
//            return;

//        int index = Mathf.Clamp(usedItems.Count, 0, puzzleSprites.Length - 1);

//        puzzleImage.sprite = puzzleSprites[index];
//    }

//    public void ForceClearPuzzle()
//    {
//        if (gameObject.activeSelf)
//        {
//            usedItems.Clear();
//            PuzzleManager.Instance.MarkCleared(puzzleID);
//            gameObject.SetActive(false);

//            if (creatureObject != null)
//                creatureObject.SetActive(false);

//            if (InventoryUIManager.Instance != null)
//                InventoryUIManager.Instance.SetPuzzleMode(false, null);

//            // 퍼즐 모드 끄고 매직 패널 닫기
//            if (MagicManager.Instance != null)
//            {
//                MagicManager.Instance.SetPuzzleMode(false);
//                //MagicManager.Instance.CloseMagicPanel();
//            }

//            Debug.Log("SurgeryRoom0Puzzle 강제 클리어 처리");
//        }
//    }

//}
