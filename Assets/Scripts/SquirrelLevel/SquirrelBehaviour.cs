using UnityEngine;

public class SquirrelBehaviour : TargetBasedSteerBehaviour
{
    [SerializeField] private float jumpProbability = 0.1f;
    [SerializeField] private float jumpCooldown = 1f;
    [SerializeField] private float pauseBeforeGoToTarget = 1f;

    public GameObject Target
    {
        set
        {
            target = value;
            if (value != null && value.CompareTag(Constants.Tags.NutBucket))
            {
                steerTowardsTargetTimer = pauseBeforeGoToTarget;
            }
        }
    }
    private float steerTowardsTargetTimer;
    private float jumpCooldownTimer = 0f;
    private Animator animator;

    protected override void Start()
    {
        base.Start();
        animator = GetComponentInChildren<Animator>();
    }

    protected override bool ShouldMove()
    {
        return steerTowardsTargetTimer <= 0 && base.ShouldMove();
    }

    protected override void Jump()
    {
        jumpCooldownTimer = jumpCooldown;
        base.Jump();
    }

    protected override void FixedUpdate()
    {
        if (jumpCooldownTimer > 0)
        {
            jumpCooldownTimer -= Time.fixedDeltaTime;
        }
        if (steerTowardsTargetTimer > 0)
        {
            steerTowardsTargetTimer -= Time.fixedDeltaTime;
        }
        base.FixedUpdate();
        animator.SetBool(Constants.AnimatorState.IsGrounded, isGrounded);
        if (steerDirection != Vector3.zero )
        {
            animator.SetBool(Constants.AnimatorState.IsWalking, true);
        }
        else
        {
            animator.SetBool(Constants.AnimatorState.IsWalking, false);
        }
    }

    protected override bool ShouldJump()
    {
        return jumpCooldownTimer <= 0f && Random.value < jumpProbability;
    }
}
