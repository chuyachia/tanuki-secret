using UnityEngine;

public class SquirrelBehaviour : TargetBasedSteerBehaviour
{
    [SerializeField] private float jumpProbability = 0.1f;
    [SerializeField] private float jumpCooldown = 5f;
    [SerializeField] private float pauseBeforeGoToTarget = 1f;
    [SerializeField] private float _targetReachedThreshold = 2f;


    public GameObject Target
    {
        set
        {
            if (value == null)
            {
                targetPosition = transform.position;
            }
            else
            {
                targetPosition = value.transform.position;
                if (value.CompareTag(Constants.Tags.NutBucket))
                {
                    steerTowardsTargetTimer = pauseBeforeGoToTarget;
                }
            }
        }
    }

    public Vector3 StayAtPoint
    {
        set
        {
            targetPosition = value;
        }

    }

    public float TargetReachedThreshold
    {
        get
        {
            return _targetReachedThreshold;
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
        return steerTowardsTargetTimer <= 0 && Utils.DistanceToTargetAboveThreshold(transform.position, targetPosition, _targetReachedThreshold);
    }

    protected override bool Jump()
    {
        if (base.Jump())
        {
            jumpCooldownTimer = Random.Range(0, jumpCooldown);
            return true;
        }
        return false;
    }

    void Update()
    {
        if (jumpCooldownTimer > 0)
        {
            jumpCooldownTimer -= Time.deltaTime;
        }
        if (steerTowardsTargetTimer > 0)
        {
            steerTowardsTargetTimer -= Time.deltaTime;
        }
    }

    protected override void FixedUpdate()
    {

        base.FixedUpdate();
        animator.SetBool(Constants.AnimatorState.IsGrounded, isGrounded);
        if (steerDirection != Vector3.zero)
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
