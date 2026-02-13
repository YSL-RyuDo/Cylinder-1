using UnityEngine;

public class PuzzleTrigger : MonoBehaviour
{
    [SerializeField] private KeyCode interactKey = KeyCode.F;

    public static bool IsPlayerNearPuzzleWall { get; private set; } = false;

    private bool playerInside = false;

    private void Update()
    {
        if (!playerInside) return;

        var ctrl = CandlePuzzleController.Instance;
        if (ctrl == null) return;

        // F를 누르면 퍼즐 진입
        if (Input.GetKeyDown(interactKey))
        {
            bool spellReady = false;

            if (DesireSpellManager.Instance != null)
            {
                spellReady = DesireSpellManager.Instance.IsSpellActivated;
            }

            ctrl.EnterPuzzleView(spellReady);
        }

        // 퍼즐 상태에서 ESC로 빠져나옴
        if (ctrl.State == CandlePuzzleController.GameState.PuzzleView &&
            Input.GetKeyDown(KeyCode.Escape))
        {
            ctrl.ExitPuzzleView();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInside = true;
        IsPlayerNearPuzzleWall = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInside = false;
        IsPlayerNearPuzzleWall = false;
    }
}

