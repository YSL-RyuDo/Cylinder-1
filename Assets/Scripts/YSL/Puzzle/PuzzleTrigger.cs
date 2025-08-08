using UnityEngine;

public class PuzzleTrigger : MonoBehaviour
{
    [Header("퍼즐 기본 설정")]
    public string puzzleID;

    protected bool isPlayerInRange = false;
    protected GameObject currentPlayer;

    protected virtual void Start()
    {
        if (PuzzleManager.Instance != null && PuzzleManager.Instance.IsCleared(puzzleID))
        {
            Debug.Log($"{puzzleID} 퍼즐은 이미 클리어되어 비활성화 처리됩니다.");
            gameObject.SetActive(false);
        }
    }

    protected virtual void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F))
        {
            int playerPersonality = GetPlayerPersonality();

            if (playerPersonality == -1)
            {
                Debug.LogWarning("플레이어 인격 정보를 얻을 수 없어 퍼즐에 진입할 수 없습니다.");
                return;
            }

            if (PuzzleManager.Instance.CanPlayerEnterPuzzle(puzzleID, playerPersonality))
            {
                PuzzleManager.Instance.OpenPuzzle(puzzleID, this);
                Debug.Log($"퍼즐 열기 시도: {puzzleID}");

                if (InventoryUIManager.Instance != null)
                    InventoryUIManager.Instance.SetPuzzleMode(true, this);
            }
            else
            {
                Debug.Log($"인격 {playerPersonality}은(는) 퍼즐 {puzzleID}에 진입할 수 없습니다.");
            }
        }
    }

    protected int GetPlayerPersonality()
    {
        if (currentPlayer == null) return -1;

        PlayerPersonality personalityComp = currentPlayer.GetComponent<PlayerPersonality>();
        if (personalityComp == null)
        {
            Debug.LogWarning("PlayerPersonality 컴포넌트를 찾을 수 없습니다.");
            return -1;
        }

        return personalityComp.GetPersonalityIndex();
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
            currentPlayer = collision.gameObject;
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
            currentPlayer = null;
        }
    }

    /// <summary>
    /// 퍼즐에 아이템 사용 시 호출. 퍼즐별로 Override 해서 구현하세요.
    /// </summary>
    /// <param name="itemId"></param>
    public virtual void TryUseItemOnPuzzle(int itemId)
    {
        Debug.LogWarning($"TryUseItemOnPuzzle 기본 구현입니다. 퍼즐별로 Override 하세요. itemId={itemId}");
    }
}
