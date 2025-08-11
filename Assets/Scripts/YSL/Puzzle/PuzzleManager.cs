using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PuzzleData
{
    public string id;
    public GameObject panel;
    public Button closeButton;
    public GameObject puzzleObject;
    public bool isCleared;

    public List<int> allowedPersonalities;

    public bool CanEnter(int playerPersonality)
    {
        if (allowedPersonalities == null || allowedPersonalities.Count == 0)
            return true;
        return allowedPersonalities.Contains(playerPersonality);
    }
}

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance;

    public PuzzleData[] puzzles;

    private HashSet<string> activePuzzles = new HashSet<string>();
    private HashSet<string> clearedPuzzles = new HashSet<string>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        foreach (var puzzle in puzzles)
        {
            if (puzzle.closeButton != null)
            {
                string idCopy = puzzle.id;
                puzzle.closeButton.onClick.AddListener(() => ClosePuzzle(idCopy, null));
            }

            if (puzzle.isCleared)
            {
                if (puzzle.panel != null)
                {
                    puzzle.panel.SetActive(false);
                }

                if (puzzle.puzzleObject != null)
                {
                    puzzle.puzzleObject.SetActive(false);
                }
            }
            else
            {
                if (puzzle.panel != null)
                {
                    puzzle.panel.SetActive(false);
                }

                if (puzzle.puzzleObject != null)
                {
                    puzzle.puzzleObject.SetActive(true);
                }
            }
        }
    }

    public bool CanPlayerEnterPuzzle(string puzzleId, int playerPersonality)
    {
        var puzzle = GetPuzzle(puzzleId);
        if (puzzle == null)
        {
            Debug.LogWarning($"퍼즐을 찾을 수 없습니다: {puzzleId}");
            return false;
        }

        bool canEnter = puzzle.CanEnter(playerPersonality);
        if (!canEnter)
        {
            Debug.Log($"플레이어 인격 {playerPersonality} 은(는) 퍼즐 {puzzleId}에 진입 불가");
        }
        return canEnter;
    }

    public void OpenPuzzle(string id, PuzzleTrigger trigger)
    {
        PuzzleData data = GetPuzzle(id);
        if (data == null) return;

        if (data.isCleared)
        {
            Debug.Log("이미 클리어한 퍼즐입니다.");
            if (data.panel != null)
                data.panel.SetActive(false);
            return;
        }

        if (data.panel != null)
            data.panel.SetActive(true);
        activePuzzles.Add(id);

        if (InventoryUIManager.Instance != null)
            InventoryUIManager.Instance.SetPuzzleMode(true, trigger);

        if (MagicManager.Instance != null)
        {
            MagicManager.Instance.SetPuzzleMode(true);
            //MagicManager.Instance.OpenMagicPanel();
        }
    }

    public void ClosePuzzle(string id, PuzzleTrigger trigger)
    {
        PuzzleData data = GetPuzzle(id);
        if (data == null) return;

        if (data.panel != null)
            data.panel.SetActive(false);
        activePuzzles.Remove(id);

        if (InventoryUIManager.Instance != null)
            InventoryUIManager.Instance.SetPuzzleMode(false, trigger);

        if (MagicManager.Instance != null)
        {
            MagicManager.Instance.SetPuzzleMode(false);
            MagicManager.Instance.CloseMagicPanel();
            Debug.Log($"{id} 퍼즐 모드 종료");
        }

        if (!data.isCleared)
            Debug.Log($"{id} 퍼즐 포기 처리됨");
        else
            Debug.Log($"{id} 퍼즐 클리어 후 닫음");
    }

    public void MarkCleared(string id)
    {
        PuzzleData data = GetPuzzle(id);
        if (data != null)
        {
            data.isCleared = true;

            if (data.puzzleObject != null)
                data.puzzleObject.SetActive(false);
        }
    }

    private PuzzleData GetPuzzle(string id)
    {
        foreach (var p in puzzles)
        {
            if (p.id == id)
                return p;
        }
        return null;
    }

    public bool IsCleared(string puzzleID)
    {
        return clearedPuzzles.Contains(puzzleID);
    }
}
