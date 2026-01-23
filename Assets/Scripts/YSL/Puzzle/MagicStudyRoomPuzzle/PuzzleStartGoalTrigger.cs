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

        if (mode == Mode.Start)
        { 
            var col = GetComponent<Collider2D>();
            Vector3 center = col != null ? (Vector3)col.bounds.center : transform.position;
            center.z = other.transform.position.z;

            manager.StartPuzzle(center);
        }
        else // Goal
        {
            if (!manager.isActive) return;
            onGoal = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (mode == Mode.Goal) onGoal = false;
    }

    void Update()
    {
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
