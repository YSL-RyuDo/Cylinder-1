using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YSortOrder : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        // Y ÁÂÇ¥°¡ ³·À»¼ö·Ï À§¿¡ ·»´õµÇµµ·Ï (-Y)
        spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
    }
}
