using UnityEngine;

public class PlayerPush : MonoBehaviour
{
    public float interactDistance = 1f;   // 플레이어 앞을 체크할 거리
    public KeyCode pushKey = KeyCode.F;   // 밀기 키 (단타 + 홀드)

    private Vector2 lastMoveDir = Vector2.down; // 마지막 이동 방향 (기본값: 아래)
    private Pushable currentPushable = null;    // 붙어있는 박스

    void Update()
    {
        // 방향키 입력 받아서 마지막 방향 갱신
        Vector2 inputDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (inputDir != Vector2.zero)
        {
            lastMoveDir = inputDir.normalized;
        }

        

        // F키 꾹 누름 → 계속 밀기
        if (Input.GetKey(pushKey))
        {
            TryHoldPush();
        }
        else
        {
            currentPushable = null; // 키에서 손 떼면 밀기 해제
        }

        //  디버그용 레이 시각화 (Scene 뷰에서 보임)
        Debug.DrawRay(transform.position, lastMoveDir * interactDistance, Color.red);
    }

    void TryHoldPush()
    {
        if (currentPushable == null)
        {
            currentPushable = DetectPushable();
        }

        if (currentPushable != null)
        {
            bool pushed = currentPushable.TryPush(lastMoveDir, transform);
            Debug.Log($"[PlayerPush] 계속 밀기 시도 → {(pushed ? "성공" : "실패")}");
        }
    }


    Pushable DetectPushable()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, lastMoveDir, interactDistance, LayerMask.GetMask("Default"));

        foreach (var hit in hits)
        {
            if (hit.collider == null) continue;
            if (hit.collider.gameObject == gameObject) continue; // 자기 자신 무시

            if (hit.collider.CompareTag("PushPuzzle"))
            {
                return hit.collider.GetComponent<Pushable>();
            }
        }
        return null;
    }
}
