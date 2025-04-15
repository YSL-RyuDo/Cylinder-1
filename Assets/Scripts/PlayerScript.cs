using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f;

    private PlayerControlls playerControlls;
    private Vector2 movement;
    private Rigidbody2D rb;

    private void Awake()
    {
        playerControlls = new PlayerControlls();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        playerControlls.Enable();
    }

    private void Update()
    {
        PlayerInput();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void PlayerInput()
    {
        movement = playerControlls.Movement.Move.ReadValue<Vector2>();
    }

    private void Move()
    {
        rb.MovePosition(rb.position + movement * (moveSpeed * Time.fixedDeltaTime));
    }

    public void Warp(float x, float y)
    {
        rb.position = new Vector2(x, y);
    }
}