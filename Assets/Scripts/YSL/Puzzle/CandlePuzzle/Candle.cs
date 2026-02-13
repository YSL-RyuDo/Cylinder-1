using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Candle : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite offSprite;
    public Sprite onSprite;

    [Header("Light (child object with Light2D)")]
    [SerializeField] private GameObject lightObj;
    [SerializeField] private Light2D light2D;

    private SpriteRenderer sr;
    private bool isLit = false;
    public bool IsLit => isLit;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (offSprite != null) sr.sprite = offSprite;

        if (lightObj == null)
        {
            light2D = GetComponentInChildren<Light2D>(true);
            if (light2D != null) lightObj = light2D.gameObject;
        }

        if (lightObj != null) lightObj.SetActive(false);
    }

    public void Light()
    {
        if (isLit) return;
        isLit = true;

        if (onSprite != null) sr.sprite = onSprite;

        if (lightObj != null) lightObj.SetActive(true);

        var lighting = FindObjectOfType<PuzzleLightingController>();
        if (lighting != null)
        {
            var all = FindObjectsOfType<Candle>();
            bool allLit = true;
            for (int i = 0; i < all.Length; i++)
            {
                if (!all[i].IsLit) { allLit = false; break; }
            }
            if (allLit) lighting.MarkAllCandlesLit();
        }
    }

    public void Unlight()
    {
        isLit = false;
        if (offSprite != null) sr.sprite = offSprite;
        if (lightObj != null) lightObj.SetActive(false);
    }
}
