using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class GriefAbilityController : MonoBehaviour
{
    [Header("Required")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform interactOrigin;

    [Header("Dash (Space)")]
    [SerializeField] private float dashSpeed = 12f;     // 1초당 이동거리 느낌
    [SerializeField] private float dashDuration = 0.12f;
    [SerializeField] private float dashCooldown = 0.5f;

    [Header("Shift Tap/Hold")]
    [Tooltip("쉬프트를 이 시간보다 짧게 누르면 '부수기', 길게 누르면 '밀기'")]
    [SerializeField] private float holdThreshold = 0.35f;

    [Header("Interact Target")]
    [SerializeField] private float interactRange = 1.2f;
    [SerializeField] private float interactRadius = 0.25f;
    [SerializeField] private LayerMask interactMask;

    [Header("Push")]
    [SerializeField] private float pushForce = 6f;

    [Header("Swim (in water)")]
    [SerializeField] private float swimSpeed = 4f;
    [SerializeField] private float waterDrag = 6f;

    private bool isDashing;
    private float dashTimer;
    private float dashCdTimer;

    private bool inWater;
    private float defaultDrag;
    private float defaultGravityScale;

    private bool shiftHeld;
    private float shiftDownTime;

    private Vector2 lastMoveDir = Vector2.right;
    private Vector2 dashDir = Vector2.right;

    // PlayerMove에서 체크할 값
    public bool IsDashingNow => isDashing;

    private void Reset()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
        defaultDrag = rb.drag;
        defaultGravityScale = rb.gravityScale;
    }

    private void Update()
    {
        
        if (!IsGriefActive()) return;

        dashCdTimer -= Time.deltaTime;

        // 방향 기록
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        Vector2 move = new Vector2(x, y);

        if (move.sqrMagnitude > 0.01f)
            lastMoveDir = move.normalized;

        // Dash
        if (Input.GetKeyDown(KeyCode.Space))
            TryDash();

        // Shift tap / hold
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            shiftDownTime = Time.time;
            shiftHeld = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            if (!shiftHeld) return;
            shiftHeld = false;

            float held = Time.time - shiftDownTime;
            if (held < holdThreshold) TryBreak();
            else TryPush();
        }

        // 애니 훅
        if (animator)
        {
            animator.SetBool("isSwimming", inWater);
            animator.SetBool("isDashing", isDashing);
        }
    }

    private void FixedUpdate()
    {
        if (!IsGriefActive()) return;

        // 핵심: transform 이동 프로젝트에서 확실히 보이게 MovePosition으로 대쉬
        if (isDashing)
        {
            dashTimer -= Time.fixedDeltaTime;

            Vector2 next = rb.position + dashDir * dashSpeed * Time.fixedDeltaTime;
            rb.MovePosition(next);

            if (dashTimer <= 0f)
                EndDash();

            return;
        }

        
    }

    private bool IsGriefActive()
    {
        return PlayerPersonality.Instance != null &&
               PlayerPersonality.Instance.currentPersonality == PersonalityType.grief;
    }

    private void TryDash()
    {
        if (dashCdTimer > 0f) return;
        if (rb == null) return;

        // 대쉬 방향 확정
        dashDir = (lastMoveDir.sqrMagnitude > 0.01f) ? lastMoveDir : Vector2.right;

        isDashing = true;
        dashTimer = dashDuration;
        dashCdTimer = dashCooldown;

        if (animator) animator.SetTrigger("dash");
        
    }

    private void EndDash()
    {
        isDashing = false;
        rb.velocity = Vector2.zero; // 안전 정리
    }

    // ====== Target Find ======
    private Collider2D FindInteractTarget()
    {
        Vector2 origin = interactOrigin ? (Vector2)interactOrigin.position : rb.position;
        Vector2 dir = (lastMoveDir.sqrMagnitude > 0.01f) ? lastMoveDir : Vector2.right;

        RaycastHit2D hit = Physics2D.CircleCast(origin, interactRadius, dir, interactRange, interactMask);
        return hit.collider;
    }

    private void TryBreak()
    {
        Collider2D col = FindInteractTarget();
        if (!col) return;

        var breakable = col.GetComponentInParent<IGriefBreakable>();
        if (breakable != null)
        {
            breakable.GriefBreak();
            if (animator) animator.SetTrigger("break");
        }
    }

    private void TryPush()
    {
        Collider2D col = FindInteractTarget();
        if (!col) return;

        var pushable = col.GetComponentInParent<IGriefPushable>();
        if (pushable != null)
        {
            pushable.GriefPush(lastMoveDir, pushForce);
            if (animator) animator.SetTrigger("push");
        }
    }

    // ====== Water hooks ======
    public void EnterWater()
    {
        if (!IsGriefActive()) return;

        inWater = true;
        rb.drag = waterDrag;
        rb.gravityScale = 0f;
    }

    public void ExitWater()
    {
        inWater = false;
        rb.drag = defaultDrag;
        rb.gravityScale = defaultGravityScale;
    }
}