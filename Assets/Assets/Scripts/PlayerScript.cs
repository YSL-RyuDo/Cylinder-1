using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public float Speed;
    private Rigidbody2D rb;

    public Collider2D BG_Area;

    private float hor;
    private float ver;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        hor = Input.GetAxis("Horizontal");
        ver = Input.GetAxis("Vertical");
    }

    void FixedUpdate()
    {
        Vector2 targetVelocity = new Vector2(hor * Speed, ver * Speed * 0.7f);
        Vector2 nextPos = rb.position + targetVelocity * Time.fixedDeltaTime;

        Bounds playerBounds = GetComponent<Collider2D>().bounds;
        Vector2 boxSize = playerBounds.size;

        int layerMask = LayerMask.GetMask("BG");

        Collider2D hit = Physics2D.OverlapBox(nextPos, boxSize, 0f, layerMask);

        if (hit != null && hit == BG_Area)
        {
            rb.velocity = targetVelocity;

            if (hor > 0) transform.eulerAngles = new Vector3(0, 0, 0);
            else if (hor < 0) transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }
}
