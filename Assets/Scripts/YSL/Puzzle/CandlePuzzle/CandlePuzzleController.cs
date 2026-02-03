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

    [Tooltip("퍼즐 중에 꺼야 하는 스크립트가 붙어있는 오브젝트들을 넣으세요. (예: Main Camera, Player)")]
    [SerializeField] private GameObject[] disableObjectsOnPuzzle;

    [Header("Cursor")]
    [SerializeField] private Texture2D normalCursor;
    [SerializeField] private Texture2D puzzleCursor;
    [SerializeField] private Vector2 cursorHotspot = Vector2.zero;

    private Vector3 camOriginalPos;
    private float camOriginalSize;
    private Coroutine cameraRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (mainCam == null) mainCam = Camera.main;
    }

    private void Start()
    {
        ApplyCursor(normalCursor);
    }

    public void EnterPuzzleView()
    {
        if (State == GameState.PuzzleView) return;

        State = GameState.PuzzleView;

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

        State = GameState.Exploration;

        if (playerMove != null) playerMove.enabled = true;
        SetDisableObjects(true);

        ApplyCursor(normalCursor);
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

                if (s is CandlePuzzleController) continue;
                if (s is PuzzleTrigger) continue;
                if (s is PuzzleClickRaycaster2D) continue;

                s.enabled = enable;
            }
        }
    }

    private void MoveCameraToPuzzle()
    {
        if (puzzleCameraTarget == null) return;

        if (cameraRoutine != null) StopCoroutine(cameraRoutine);
        cameraRoutine = StartCoroutine(CameraTransition(
            mainCam.transform.position,
            new Vector3(puzzleCameraTarget.position.x, puzzleCameraTarget.position.y, mainCam.transform.position.z),
            mainCam.orthographicSize,
            puzzleOrthoSize,
            cameraMoveTime
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
            cameraMoveTime
        ));
    }

    private IEnumerator CameraTransition(Vector3 fromPos, Vector3 toPos, float fromSize, float toSize, float time)
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
}


