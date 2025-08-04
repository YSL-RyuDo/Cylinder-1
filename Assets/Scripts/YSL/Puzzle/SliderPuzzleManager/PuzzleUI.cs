using UnityEngine;

public class PuzzleUI : MonoBehaviour
{
    public SliderPuzzleManager puzzleManager;

    public void OnUp() => puzzleManager.TryMove(Vector2Int.up);
    public void OnDown() => puzzleManager.TryMove(Vector2Int.down);
    public void OnLeft() => puzzleManager.TryMove(Vector2Int.left);
    public void OnRight() => puzzleManager.TryMove(Vector2Int.right);
}
