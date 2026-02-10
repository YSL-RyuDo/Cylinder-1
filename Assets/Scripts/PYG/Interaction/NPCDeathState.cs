using UnityEngine;

public class NPCDeathState : MonoBehaviour
{
    public bool isDead = true;

    public Sprite aliveSprite;
    public Sprite deadSprite; 

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        UpdateSprite();
    }

    public void Revive()
    {
        isDead = false;
        UpdateSprite();
        Debug.Log($"[NPCDeathState] {name} is now ALIVE!");
    }

    private void UpdateSprite()
    {
        if (sr == null) return;

        if (isDead)
        {
            if (deadSprite != null) sr.sprite = deadSprite;
        }
        else
        {
            if (aliveSprite != null) sr.sprite = aliveSprite;
        }
    }
}
