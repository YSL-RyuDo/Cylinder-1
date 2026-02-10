using System.Collections;
using UnityEngine;

public class CandlePuzzleController : MonoBehaviour
{
    public static CandlePuzzleController Instance { get; private set; }

    public enum GameState { Exploration, PuzzleView }
    public GameState State { get; private set; } = GameState.Exploration;

    [Header("References")]
    [SerializeField] private Camera mainCam;
    [SerializeField] private Transform puzzleCameraTarget;
    [SerializeField] private float puzzleOrthoSize = 10f;
    [SerializeField] private float cameraMoveTime = 0.35f;

    [Header("Player Control (disable)")]
    [SerializeField] private PlayerMove playerMove;

    [Tooltip("퍼즐 중에 꺼야 하는 '스크립트가 붙어있는 오브젝트'들")]
    [SerializeField] private GameObject[] disableObjectsOnPuzzle;

    [Header("Cursor")]
    [SerializeField] private Texture2D normalCursor;
    [SerializeField] private Texture2D puzzleCursor;
    [SerializeField] private Vector2 cursorHotspot = Vector2.zero;

    private Vector3 camOriginalPos;
    private float camOriginalSize;
    private Coroutine cameraRoutine;

    private CameraFollow camFollow;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (mainCam == null) mainCam = Camera.main;
        if (mainCam != null) camFollow = mainCam.GetComponent<CameraFollow>();
    }

    private void Start()
    {
        ApplyCursor(normalCursor);

        // ✅ 핵심: "상호작용 전 검정 화면" 방지
        // CameraFollow가 아직 맵 바운더리를 못 잡아서 카메라가 멀리 있어도,
        // SnapToPositionImmediate는 boundary 없어도 플레이어 위치로 카메라를 옮겨줌.
        TrySnapCameraToPlayer();
    }

    private void LateUpdate()
    {
        // 퍼즐 중엔 손대지 않음
        if (State != GameState.Exploration) return;

        if (mainCam == null) return;
        if (camFollow == null) return;
        if (camFollow.target == null) return;

        // ✅ 핵심: CameraFollow가 return 타는 상황(바운더리 둘 다 null)일 때만 임시 팔로우
        if (playerMove != null &&
            playerMove.CurrentMapBoundary == null &&
            playerMove.CurrentCircularBoundary == null)
        {
            Vector3 tp = camFollow.target.position;
            mainCam.transform.position = new Vector3(tp.x, tp.y, mainCam.transform.position.z);
        }
    }


    private void TrySnapCameraToPlayer()
    {
        if (camFollow == null) return;
        if (camFollow.target == null) return;

        // 외부제어가 켜져있으면 풀고(혹시 씬 이동/재진입에서 꼬이는 경우 대비)
        camFollow.EndExternalControl();

        // boundary 없어도 일단 플레이어를 보게는 만들기
        camFollow.SnapToPositionImmediate(camFollow.target.position);
    }

    public void EnterPuzzleView()
    {
        if (State == GameState.PuzzleView) return;
        if (mainCam == null || puzzleCameraTarget == null) return;

        State = GameState.PuzzleView;

        // 퍼즐 동안 Follow가 덮어쓰기 금지
        camFollow?.BeginExternalControl();

        if (playerMove != null) playerMove.enabled = false;

        SetDisableObjects(false);
        ApplyCursor(puzzleCursor);

        camOriginalPos = mainCam.transform.position;
        camOriginalSize = mainCam.orthographicSize;

        MoveCameraToPuzzle();
    }

    public void ExitPuzzleView()
    {
        if (State == GameState.Exploration) return;
        if (mainCam == null) return;

        State = GameState.Exploration;

        if (playerMove != null) playerMove.enabled = true;
        SetDisableObjects(true);

        ApplyCursor(normalCursor);

        // ✅ EndExternalControl은 "복귀 이동 끝난 뒤"에 (아래 MoveCameraBack 콜백에서)
        MoveCameraBack();
    }

    private void SetDisableObjects(bool enable)
    {
        if (disableObjectsOnPuzzle == null) return;

        for (int i = 0; i < disableObjectsOnPuzzle.Length; i++)
        {
            GameObject go = disableObjectsOnPuzzle[i];
            if (go == null) continue;

            var scripts = go.GetComponents<MonoBehaviour>();
            foreach (var s in scripts)
            {
                if (s == null) continue;
                if (s == this) continue;

                // 퍼즐 관련은 항상 살아있어야 함
                if (s is CandlePuzzleController) continue;
                if (s is PuzzleTrigger) continue;
                if (s is PuzzleClickRaycaster2D) continue;

                s.enabled = enable;
            }
        }
    }

    private void MoveCameraToPuzzle()
    {
        if (cameraRoutine != null) StopCoroutine(cameraRoutine);

        cameraRoutine = StartCoroutine(CameraTransition(
            mainCam.transform.position,
            new Vector3(puzzleCameraTarget.position.x, puzzleCameraTarget.position.y, mainCam.transform.position.z),
            mainCam.orthographicSize,
            puzzleOrthoSize,
            cameraMoveTime,
            onDone: null
        ));
    }

    private void MoveCameraBack()
    {
        if (cameraRoutine != null) StopCoroutine(cameraRoutine);

        cameraRoutine = StartCoroutine(CameraTransition(
            mainCam.transform.position,
            camOriginalPos,
            mainCam.orthographicSize,
            camOriginalSize,
            cameraMoveTime,
            onDone: () =>
            {
                // ✅ 복귀 끝난 뒤에 Follow 다시 켜기
                camFollow?.EndExternalControl();

                // ✅ 복귀 직후 한 번 더 스냅 (Follow가 boundary 못 잡은 맵에서도 검정 방지)
                TrySnapCameraToPlayer();
            }
        ));
    }

    private IEnumerator CameraTransition(
        Vector3 fromPos, Vector3 toPos,
        float fromSize, float toSize,
        float time,
        System.Action onDone)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.0001f, time);
            mainCam.transform.position = Vector3.Lerp(fromPos, toPos, t);
            mainCam.orthographicSize = Mathf.Lerp(fromSize, toSize, t);
            yield return null;
        }

        mainCam.transform.position = toPos;
        mainCam.orthographicSize = toSize;
        cameraRoutine = null;

        onDone?.Invoke();
    }

    private void ApplyCursor(Texture2D tex)
    {
        if (tex == null)
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            return;
        }
        Cursor.SetCursor(tex, cursorHotspot, CursorMode.Auto);
    }

    // ✅ (선택) 다른 스크립트에서 카메라 참조가 필요하면 안전하게 제공
    public Camera GetMainCamera() => mainCam;
}


