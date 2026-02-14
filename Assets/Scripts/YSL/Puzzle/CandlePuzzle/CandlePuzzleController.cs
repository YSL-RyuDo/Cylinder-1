using System.Collections;
using UnityEngine;

public class CandlePuzzleController : MonoBehaviour
{
    public static CandlePuzzleController Instance { get; private set; }

    public enum GameState { Exploration, PuzzleView }
    public GameState State { get; private set; } = GameState.Exploration;

    public bool CandleInteractionUnlocked { get; private set; }
    public void UnlockCandleInteraction() => CandleInteractionUnlocked = true;

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

    [Header("Lighting")]
    [SerializeField] private PuzzleLightingController lighting;

    private Vector3 camOriginalPos;
    private float camOriginalSize;
    private Coroutine cameraRoutine;

    private CameraFollow camFollow;
    private PlayerCastingController casting;

    private bool isExiting = false;
    private bool wasCastingThisPuzzle = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (mainCam == null) mainCam = Camera.main;
        if (mainCam != null) camFollow = mainCam.GetComponent<CameraFollow>();

        casting = FindObjectOfType<PlayerCastingController>();
    }

    private void Start()
    {
        ApplyCursor(normalCursor);
        TrySnapCameraToPlayer();
    }

    private void LateUpdate()
    {
        if (isExiting) return;

        if (State != GameState.Exploration) return;
        if (mainCam == null || camFollow == null || camFollow.target == null) return;

        if (playerMove != null &&
            playerMove.CurrentMapBoundary == null &&
            playerMove.CurrentCircularBoundary == null)
        {
            Vector3 tp = camFollow.target.position;
            mainCam.transform.position = new Vector3(tp.x, tp.y, mainCam.transform.position.z);
        }
    }

    public void EnterPuzzleView(bool spellReady)
    {
        if (State == GameState.PuzzleView) return;
        if (mainCam == null || puzzleCameraTarget == null) return;

        State = GameState.PuzzleView;
        CandleInteractionUnlocked = false;
        isExiting = false;

        wasCastingThisPuzzle = spellReady;

        lighting?.OnEnterPuzzle();

        camFollow?.BeginExternalControl();
        if (playerMove != null) playerMove.enabled = false;

        SetDisableObjects(false);

        ApplyCursor(spellReady ? puzzleCursor : normalCursor);

        if (spellReady)
            (casting ??= FindObjectOfType<PlayerCastingController>())?.StartCastingMotionOnly();

        camOriginalPos = mainCam.transform.position;
        camOriginalSize = mainCam.orthographicSize;

        MoveCameraToPuzzle();
    }

    public void ExitPuzzleView()
    {
        if (State == GameState.Exploration) return;
        if (mainCam == null) return;
        if (isExiting) return;

        StartCoroutine(ExitPuzzleViewRoutine());
    }

    private IEnumerator ExitPuzzleViewRoutine()
    {
        isExiting = true;

        lighting?.OnExitPuzzle();

        if (playerMove != null) playerMove.enabled = true;
        SetDisableObjects(true);

        ApplyCursor(normalCursor);

        casting ??= FindObjectOfType<PlayerCastingController>();

        if (wasCastingThisPuzzle && casting != null)
        {
            casting.SetCasting(false);

            float wait = 0.25f;

            var spriteAnim = casting.GetComponentInChildren<PlayerCastSpriteAnimator>();
            if (spriteAnim != null)
            {
                try
                {
                    wait = Mathf.Max(0.15f, spriteAnim.GetEndDuration());
                }
                catch
                {
                    wait = 0.35f;
                }
            }
            else
            {
                wait = 0.35f;
            }

            yield return new WaitForSeconds(wait);
        }
        else
        {
            yield return null;
        }

        MoveCameraBack();

        while (cameraRoutine != null)
            yield return null;

        State = GameState.Exploration;

        isExiting = false;
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

                if (s is PuzzleTrigger) continue;
                if (s is PuzzleClickRaycaster2D) continue;

                s.enabled = enable;
            }
        }
    }

    private void MoveCameraToPuzzle()
    {
        StartCameraTransition(
            mainCam.transform.position,
            new Vector3(puzzleCameraTarget.position.x, puzzleCameraTarget.position.y, mainCam.transform.position.z),
            mainCam.orthographicSize,
            puzzleOrthoSize,
            null
        );
    }

    private void MoveCameraBack()
    {
        StartCameraTransition(
            mainCam.transform.position,
            camOriginalPos,
            mainCam.orthographicSize,
            camOriginalSize,
            () =>
            {
                camFollow?.EndExternalControl();
                TrySnapCameraToPlayer();
            }
        );
    }

    private void StartCameraTransition(
        Vector3 fromPos, Vector3 toPos,
        float fromSize, float toSize,
        System.Action onDone)
    {
        if (cameraRoutine != null) StopCoroutine(cameraRoutine);

        cameraRoutine = StartCoroutine(CameraTransition(
            fromPos, toPos,
            fromSize, toSize,
            cameraMoveTime,
            onDone
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

    private void TrySnapCameraToPlayer()
    {
        if (camFollow == null || camFollow.target == null) return;

        camFollow.EndExternalControl();
        camFollow.SnapToPositionImmediate(camFollow.target.position);
    }

    private void ApplyCursor(Texture2D tex)
    {
        Cursor.SetCursor(tex, cursorHotspot, CursorMode.Auto);
    }

    public Camera GetMainCamera() => mainCam;
}
