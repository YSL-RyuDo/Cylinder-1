using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderPuzzleManager : MonoBehaviour
{
    [Header("UI Settings")]
    public RectTransform boardRoot;
    public float tileSize = 100f;

    [Header("Tile Prefabs")]
    public TilePrefabs tilePrefabs;

    [Header("CSV Map File")]
    public TextAsset csvFile;

    private int width;
    private int height;
    private int[,] grid;
    private Vector2Int playerPos;
    private Vector2Int goalPos;
    private RectTransform playerVisual;

    private bool isMoving = false;
    private Vector2 moveStartPos;
    private Vector2 moveTargetPos;
    private float moveDuration = 0.3f;
    private float moveElapsed = 0f;

    private List<string> mapLines = new List<string>();

    private void OnEnable()
    {
        InitializePuzzle();
    }

    private void InitializePuzzle()
    {
        LoadCSV();
        LoadMapFromCSV();

        foreach (Transform child in boardRoot)
        {
            Destroy(child.gameObject);
        }

        if (playerVisual != null)
        {
            Destroy(playerVisual.gameObject);
            playerVisual = null;
        }

        GenerateBoardVisual();
        CreatePlayerVisual();

        isMoving = false;
    }

    private void Update()
    {
        if (isMoving)
        {
            moveElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(moveElapsed / moveDuration);
            playerVisual.anchoredPosition = Vector2.Lerp(moveStartPos, moveTargetPos, t);

            if (t >= 1f)
            {
                isMoving = false;
            }
        }
    }
    private void LoadCSV()
    {
        mapLines.Clear();

        using (StringReader reader = new StringReader(csvFile.text))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                mapLines.Add(line);
            }
        }
    }

    void LoadMapFromCSV()
    {
        if (mapLines.Count == 0)
        {
            Debug.LogError("CSV 파일이 비어있거나 로드 안됨");
            return;
        }

        height = mapLines.Count;
        width = mapLines[0].Split(',').Length;

        grid = new int[height, width];

        for (int y = 0; y < height; y++)
        {
            string[] cells = mapLines[height - 1 - y].Split(',');

            for (int x = 0; x < width; x++)
            {
                int tileValue;
                if (!int.TryParse(cells[x], out tileValue))
                {
                    Debug.LogWarning($"맵 데이터 파싱 실패: {cells[x]} (위치 {x},{y})");
                    tileValue = 1;
                }

                grid[y, x] = tileValue;

                if (tileValue == 3)
                    playerPos = new Vector2Int(x, y);
                else if (tileValue == 4)
                    goalPos = new Vector2Int(x, y);
            }
        }
    }

    void GenerateBoardVisual()
    {
        Vector2 boardOrigin = new Vector2(-(width * tileSize) / 2f - 100f, -(height * tileSize) / 2f);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int val = grid[y, x];
                GameObject prefab = GetTilePrefabByValue(val);
                if (prefab == null) continue;

                GameObject tile = Instantiate(prefab, boardRoot);
                RectTransform rt = tile.GetComponent<RectTransform>();
                rt.anchoredPosition = boardOrigin + new Vector2(x * tileSize, y * tileSize);
            }
        }
    }

    void CreatePlayerVisual()
    {
        GameObject playerObj = Instantiate(tilePrefabs.player, this.transform);
        playerVisual = playerObj.GetComponent<RectTransform>();
        playerVisual.name = "PlayerVisual";

        // 위치 설정 (기존대로)
        Vector2 boardOrigin = new Vector2(-(width * tileSize) / 2f - 100f, -(height * tileSize) / 2f);
        playerVisual.anchoredPosition = boardOrigin + new Vector2(playerPos.x * tileSize, playerPos.y * tileSize);
    }


    void StartMoveVisual(Vector2Int gridPos)
    {
        Vector2 boardOrigin = new Vector2(-(width * tileSize) / 2f - 100f, -(height * tileSize) / 2f);
        moveStartPos = playerVisual.anchoredPosition;
        moveTargetPos = boardOrigin + new Vector2(gridPos.x * tileSize, gridPos.y * tileSize);
        moveElapsed = 0f;
        isMoving = true;
    }

    public void TryMove(Vector2Int dir)
    {
        if (isMoving) return;

        Vector2Int newPos = Slide(playerPos, dir);
        if (newPos == playerPos) return;

        playerPos = newPos;
        StartMoveVisual(playerPos);

        if (playerPos == goalPos)
        {
            Debug.Log("퍼즐 성공!");
        }
    }

    Vector2Int Slide(Vector2Int pos, Vector2Int dir)
    {
        Vector2Int cur = pos;

        while (true)
        {
            Vector2Int next = cur + dir;
            if (next.x < 0 || next.x >= width || next.y < 0 || next.y >= height)
                break;

            if (grid[next.y, next.x] == 1 || grid[next.y, next.x] == 2)
                break;

            cur = next;
        }

        return cur;
    }

    GameObject GetTilePrefabByValue(int val)
    {
        switch (val)
        {
            case 0: return tilePrefabs.oTile;   // O
            case 1: return tilePrefabs.xTile;   // X (벽)
            case 2: return tilePrefabs.cTile;   // C (가림막)
            case 3: return tilePrefabs.aTile;   // A (시작)
            case 4: return tilePrefabs.bTile;   // B (목표)
            default: return null;
        }
    }
}
