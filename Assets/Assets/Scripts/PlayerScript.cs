using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public float Speed;
    public float verSpeed;
    private Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {

    }

    private void FixedUpdate()
    {
        MovePlayer();
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        
    }

    private void MovePlayer()
    {
        //키 입력
        float ver = Input.GetAxis("Vertical");
        float hor = Input.GetAxis("Horizontal");
        //위치 이동
        rb.velocity = new Vector2(hor * Speed, verSpeed * ver * Speed);

        //회전
        if (hor > 0)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else if (hor < 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
    }

}