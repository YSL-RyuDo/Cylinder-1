using System.Collections;
using UnityEngine;
using TMPro;

public class LibraryPuzzlePanel : MonoBehaviour
{
    [Header("Seal Groups")]
    public SealGroupUI redSeal;
    public SealGroupUI greenSeal;
    public SealGroupUI blueSeal;

    [Header("UI")]
    public TextMeshProUGUI resultText;

    [Header("Timing")]
    [SerializeField] private float glowDelay = 1.2f;
    [SerializeField] private float rotateDuration = 30f;
    private int rotationDoneCount = 0;
    private void OnEnable()
    {
        redSeal.ResetGroup();
        greenSeal.ResetGroup();
        blueSeal.ResetGroup();

        StartCoroutine(RunPuzzle());
    }

    private IEnumerator RunPuzzle()
    {
        resultText.text = "";

        SetAllRandomAnswers();
        yield return new WaitForSeconds(glowDelay);

        RotateAll();
    }


    private void SetAllRandomAnswers()
    {
        redSeal.SetRandomAnswer();
        greenSeal.SetRandomAnswer();
        blueSeal.SetRandomAnswer();
    }

    private void RotateAll()
    {
        rotationDoneCount = 0;

        redSeal.RotateGroup(rotateDuration, clockwise: true, onComplete: OnOneSealRotated);
        greenSeal.RotateGroup(rotateDuration, clockwise: false, onComplete: OnOneSealRotated);
        blueSeal.RotateGroup(rotateDuration, clockwise: true, onComplete: OnOneSealRotated);
    }

    private void OnOneSealRotated()
    {
        rotationDoneCount++;

        if (rotationDoneCount >= 3)
        {
            EnableAllInputs();
        }
    }

    private void EnableAllInputs()
    {
        redSeal.EnableInput();
        greenSeal.EnableInput();
        blueSeal.EnableInput();
    }

    public void OnSealGroupAnswered()
    {
        if (AllAnswered())
        {
            if (AllCorrect())
            {
                resultText.text = "<color=green>Clear</color>";
            }
            else
            {
                resultText.text = "<color=red>Error</color>";
                Invoke(nameof(Retry), 2f);
            }
        }
    }

    private bool AllAnswered()
    {
        return redSeal.IsAnswered && greenSeal.IsAnswered && blueSeal.IsAnswered;
    }

    private bool AllCorrect()
    {
        return redSeal.IsCorrect && greenSeal.IsCorrect && blueSeal.IsCorrect;
    }

    private void Retry()
    {
        redSeal.ResetGroup();
        greenSeal.ResetGroup();
        blueSeal.ResetGroup();

        StartCoroutine(RunPuzzle());
    }
}
