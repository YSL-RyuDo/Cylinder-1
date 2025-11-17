//using UnityEngine;
//using UnityEngine.UI;

//public class SurgeryRoom1Puzzle : PuzzleTrigger
//{
//    [Header("SurgeryRoom1 퍼즐 전용")]
//    public GameObject surgeryRoomPanel;  // 퍼즐 패널 (인스펙터에서 할당)
//    public GameObject creatureObject;    // 클리어 시 비활성화할 크리쳐

//    [Header("연동 퍼즐 참조")]
//    public SurgeryRoom0Puzzle linkedPuzzle0;

//    [Header("퍼즐 클리어 버튼")]
//    public Button clearButton;  // 인스펙터에서 할당하거나, 코드에서 찾아도 됨

//    private bool isPuzzleOpen = false;

//    protected override void Start()
//    {
//        base.Start();

//        if (surgeryRoomPanel != null)
//            surgeryRoomPanel.SetActive(false);

//        if (clearButton != null)
//        {
//            clearButton.onClick.AddListener(OnPuzzleClearButtonClicked);
//        }
//    }

//    protected void OnDestroy()
//    {
//        if (clearButton != null)
//        {
//            clearButton.onClick.RemoveListener(OnPuzzleClearButtonClicked);
//        }
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

//            if (isPuzzleOpen)
//            {
//                ClosePuzzle();
//                return;
//            }

//            if (PuzzleManager.Instance.CanPlayerEnterPuzzle(puzzleID, playerPersonality))
//            {
//                OpenPuzzle();

//                //string[] enterDialogue = DialogueLoader.Instance.GetSurgeryRoomEnterDialogue(playerPersonality);
//                //DialogueManager.Instance.StartDialogue(enterDialogue, playerPersonality);
//            }
//            else
//            {
//                Debug.Log($"인격 {playerPersonality}은(는) 퍼즐 {puzzleID}에 진입할 수 없습니다.");
//            }
//        }
//    }

//    private void OpenPuzzle()
//    {
//        if (surgeryRoomPanel != null)
//            surgeryRoomPanel.SetActive(true);

//        isPuzzleOpen = true;

//        if (InventoryUIManager.Instance != null)
//            InventoryUIManager.Instance.SetPuzzleMode(true, this);

//        Debug.Log("SurgeryRoom1 퍼즐 패널 열림");
//    }

//    private void ClosePuzzle()
//    {
//        if (surgeryRoomPanel != null)
//            surgeryRoomPanel.SetActive(false);

//        isPuzzleOpen = false;

//        if (InventoryUIManager.Instance != null)
//            InventoryUIManager.Instance.SetPuzzleMode(false, null);

//        Debug.Log("SurgeryRoom1 퍼즐 패널 닫힘");
//    }

//    public override void TryUseItemOnPuzzle(int itemId)
//    {
//        Debug.LogWarning("SurgeryRoom1Puzzle에서는 아이템 사용이 필요하지 않습니다.");
//    }

//    public void OnPuzzleClearButtonClicked()
//    {
//        if (!isPuzzleOpen)
//        {
//            Debug.LogWarning("퍼즐이 열려있지 않습니다.");
//            return;
//        }

//        Debug.Log("퍼즐 클리어 버튼 클릭됨 (SurgeryRoom1Puzzle)");

//        PuzzleManager.Instance.MarkCleared(puzzleID);
//        ClosePuzzle();
//        gameObject.SetActive(false);
//        Debug.Log("퍼즐 클리어 완료, 퍼즐 오브젝트 비활성화");

//        if (creatureObject != null)
//        {
//            creatureObject.SetActive(false);
//        }

//        if (linkedPuzzle0 != null && !PuzzleManager.Instance.IsCleared(linkedPuzzle0.puzzleID))
//        {
//            linkedPuzzle0.ForceClearPuzzle();
//        }

//        PortalManager portalManager = FindObjectOfType<PortalManager>();
//        if (portalManager != null)
//        {
//            int portalIndex = 3; // 0-based 인덱스 3번째 포탈
//            portalManager.SetPortalActive(portalIndex, true);
//            Debug.Log("Portal3의 AtoB와 BtoA를 활성화했습니다.");
//        }
//        else
//        {
//            Debug.LogWarning("PortalManager를 찾을 수 없습니다.");
//        }
//    }

//    public void ForceClearPuzzle()
//    {
//        if (gameObject.activeSelf)
//        {
//            ClosePuzzle();
//            PuzzleManager.Instance.MarkCleared(puzzleID);
//            gameObject.SetActive(false);

//            if (creatureObject != null)
//                creatureObject.SetActive(false);

//            if (InventoryUIManager.Instance != null)
//                InventoryUIManager.Instance.SetPuzzleMode(false, null);

//            Debug.Log("SurgeryRoom1Puzzle 강제 클리어 처리");
//        }
//    }
//}
