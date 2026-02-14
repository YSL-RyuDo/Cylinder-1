using UnityEngine;
using System.Collections;

public class PlayerCastingController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private PlayerMove playerMove;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PlayerCastSpriteAnimator spriteAnimator;

    [Header("Optional: Animator 덮어쓰기 방지용")]
    [SerializeField] private Animator animatorToDisable;

    private bool isCasting;

    private void Awake()
    {
        if (playerMove == null) playerMove = GetComponent<PlayerMove>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (spriteAnimator == null) spriteAnimator = GetComponent<PlayerCastSpriteAnimator>();
        if (animatorToDisable == null) animatorToDisable = GetComponent<Animator>();
    }

    public void StartCastingMotionOnly() => SetCasting(true);

    public void SetCasting(bool on)
    {
        if (isCasting == on) return;
        isCasting = on;

        playerMove?.SetMoveLocked(on);
        if (on) StopPhysics();

        if (spriteAnimator != null)
        {
            if (on)
            {
                if (animatorToDisable != null) animatorToDisable.enabled = false;
                spriteAnimator.BeginCast();
            }
            else
            {
                if (animatorToDisable != null) animatorToDisable.enabled = false;

                spriteAnimator.EndCast();

                float t = spriteAnimator.GetEndDuration();
                if (t <= 0f) t = 0.15f;
                StartCoroutine(ReEnableAnimatorAfter(t));
            }
        }
    }

    private IEnumerator ReEnableAnimatorAfter(float t)
    {
        yield return new WaitForSeconds(t);
        if (animatorToDisable != null) animatorToDisable.enabled = true;
    }

    public void StopCastingMotionOnly()
    {
        spriteAnimator?.SnapToStanding();

        if (animatorToDisable != null)
            animatorToDisable.enabled = true;

        isCasting = false;
        playerMove?.SetMoveLocked(false);

        StopPhysics();
    }

    private void StopPhysics()
    {
        if (rb == null) return;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }
}
