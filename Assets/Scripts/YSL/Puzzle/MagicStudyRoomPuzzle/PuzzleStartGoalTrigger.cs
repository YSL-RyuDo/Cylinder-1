using UnityEngine;

public class PuzzleStartGoalTrigger : MonoBehaviour
{
    public enum Mode { Start, Goal }
    public Mode mode;

    public MagicStudyPuzzleManager manager;

    private bool onGoal = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (manager == null) return;

        // ✅ 퍼즐 진행 중이면 Start/Goal 상관없이 즉시 초기화
        if (manager.isActive)
        {
            manager.Fail_ReturnPlatform(); // 책들이 다시 올라오며 초기화
            return;
        }

        // ✅ 퍼즐 비활성 상태에서 Start를 밟으면 퍼즐 시작
        if (mode == Mode.Start)
        {
            var col = GetComponent<Collider2D>();
            Vector3 center = col != null ? (Vector3)col.bounds.center : transform.position;
            center.z = other.transform.position.z;

            manager.StartPuzzle(center);
        }
        else // Goal인데 isActive=false면 아무것도 안 함
        {
            onGoal = false;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (mode == Mode.Goal) onGoal = false;
    }

    void Update()
    {
        // (남겨둔 기존 E키 로직: 하지만 위에서 Goal 밟는 순간 초기화되니 실사용은 거의 없음)
        if (mode != Mode.Goal) return;
        if (!onGoal) return;
        if (manager == null || !manager.isActive) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (manager.IsAllTilesStepped())
                manager.ClearPuzzle();
            else
                manager.Fail_ReturnPlatform();
        }
    }
}
