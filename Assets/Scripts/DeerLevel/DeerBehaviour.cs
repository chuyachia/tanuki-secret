using UnityEngine;

public abstract class DeerBehaviour : TargetBasedSteerBehaviour
{
    [SerializeField] protected float targetReachedSquaredDistance = 4f;
    [SerializeField] protected float wanderRadius;
    [SerializeField] protected float wanderStopDuration = 2f;
    [SerializeField] protected float wanderSpeed = 2f;

    protected State state;
    protected Animator animator;
    private float wanderStopTimer;
    private bool isWanderStopping;
    protected bool targetReached;
    private float speedDecrement;
    private WanderTargetGenerator targetGenerator;

    protected enum State
    {
        Waiting,
        Migrating,
        Wandering
    }

    protected override void Start()
    {
        base.Start();
        animator = GetComponentInChildren<Animator>();
        state = State.Waiting;
        EventManager.Instance.RegisterDeerLevelEventListener(HandleEvent);
    }

    void OnDestroy()
    {
        EventManager.Instance.UnregisterDeerLevelEventListener(HandleEvent);
    }

    public void HandleEvent(GameObject[] gameObjects, EventManager.DeerLevelEvent eventType)
    {
        if (eventType == EventManager.DeerLevelEvent.StartJourney)
        {
            state = State.Migrating;
        }
        if (eventType == EventManager.DeerLevelEvent.ArriveAtDestination)
        {
            state = State.Wandering;
            targetGenerator = new WanderTargetGenerator(wanderRadius, transform.position);
            targetPosition = targetGenerator.GetNewTarget();
            animator.SetBool(Constants.AnimatorState.IsRunning, false);
        }
    }

    public void DecreseSpeed(float decrement)
    {
        speedDecrement += decrement;
    }


    protected override float GetSpeed()
    {
        return state == State.Migrating ? speed - speedDecrement : wanderSpeed;
    }

    protected override void FixedUpdate()
    {

        switch (state)
        {
            case State.Waiting:
                {
                    targetPosition = transform.position;
                    break;
                }
            case State.Migrating:
                {
                    targetPosition = GetMigrationTarget();
                    break;
                }
            case State.Wandering:
                {
                    if (isWanderStopping)
                    {
                        wanderStopTimer -= Time.deltaTime;
                        if (wanderStopTimer <= 0)
                        {
                            isWanderStopping = false;
                            targetPosition = targetGenerator.GetNewTarget();
                        }
                    }
                    else if (targetReached)
                    {
                        isWanderStopping = true;
                        targetPosition = transform.position;
                        wanderStopTimer = Random.Range(0f, wanderStopDuration);
                    }
                    break;
                }
        }
        targetReached = Utils.DistanceToTargetWithinThreshold(transform.position, targetPosition, targetReachedSquaredDistance);
        base.FixedUpdate();
        if (steerDirection != Vector3.zero)
        {
            animator.SetBool(Constants.AnimatorState.IsWalking, true);
            if (state == State.Migrating)
            {
                animator.SetBool(Constants.AnimatorState.IsRunning, true);
            }
            else
            {
                animator.SetBool(Constants.AnimatorState.IsRunning, false);
            }
        }
        else
        {
            animator.SetBool(Constants.AnimatorState.IsWalking, false);
            animator.SetBool(Constants.AnimatorState.IsRunning, false);
        }
    }

    protected override bool ShouldMove()
    {
        return !targetReached;
    }

    protected abstract Vector3 GetMigrationTarget();
}