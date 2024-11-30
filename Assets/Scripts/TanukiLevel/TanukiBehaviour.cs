using UnityEngine;

public class TanukiBehaviour : TargetBasedSteerBehaviour
{
    [SerializeField] private float stopDuration = 2f;
    [SerializeField] private State state;
    [SerializeField] private float wanderRadius = 2f;
    [SerializeField] protected float targetReachedSquaredDistance = 4f;
    private Animator animator;
    private bool isStopping;
    private float stopTimer;
    private WanderTargetGenerator targetGenerator;
    private bool targetReached;

    public enum State
    {
        Idle, Wander, FollowWaypoints
    }

    protected override void Start()
    {
        base.Start();
        animator = GetComponentInChildren<Animator>();
        targetGenerator = new WanderTargetGenerator(wanderRadius, transform.position);
        targetPosition = transform.position;
    }

    private void setTargetPosition()
    {
        if (state.Equals(State.Idle))
        {
            targetPosition = transform.position;
        }
        else
        {
            if (isStopping)
            {
                stopTimer -= Time.deltaTime;
                if (stopTimer <= 0)
                {
                    isStopping = false;
                    targetPosition = targetGenerator.GetNewTarget();
                }
            }
            else
            {
                targetReached = Utils.DistanceToTargetWithinThreshold(transform.position, targetPosition, targetReachedSquaredDistance);
                if (targetReached)
                {
                    targetPosition = transform.position;
                    stopTimer = Random.Range(0f, stopDuration);
                    isStopping = true;
                }
            }
        }
    }

    protected override void FixedUpdate()
    {
        setTargetPosition();
        base.FixedUpdate();
        animator.SetBool(Constants.AnimatorState.IsGrounded, isGrounded);
        if (steerDirection != Vector3.zero)
        {
            animator.SetBool(Constants.AnimatorState.IsWalking, true);
            animator.SetFloat(Constants.AnimatorState.speed, GetSpeed());
        }
        else
        {
            animator.SetBool(Constants.AnimatorState.IsWalking, false);
        }
    }
}
