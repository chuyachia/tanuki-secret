using UnityEngine;

public class FollowerDeerBehaviour : TargetBasedSteerBehaviour
{
    [SerializeField] private float targetReachedSquaredDistance = 4f;

    public GameObject Target
    {
        set
        {
            target = value;
        }
    }

    public DeerPositionParamters deerPositionParamters;
    private Animator animator;
    private GameObject target;
    private float speedDecrement;

    public void CaughtByWolf()
    {
        target = null;
        animator.SetBool(Constants.AnimatorState.IsAttacked, true);
    }

    public void DecreseSpeed(float decrement)
    {
        speedDecrement += decrement;
    }

    void OnEnable()
    {
        target = null;
        animator = GetComponentInChildren<Animator>();
        speedDecrement = 0f;
        animator.SetBool(Constants.AnimatorState.IsRunning, false);
    }

    protected override float GetSpeed()
    {
        return speed - speedDecrement;
    }

    protected override void Start()
    {
        base.Start();
        animator = GetComponentInChildren<Animator>();
    }

    protected override bool ShouldJump()
    {
        return false;
    }

    protected override void FixedUpdate()
    {
        if (target == null)
        {
            targetPosition = transform.position;
        }
        else
        {
            Vector3 position = target.transform.position - new Vector3(deerPositionParamters.NumberInRow * deerPositionParamters.SpacingHorizontal - (deerPositionParamters.ObjectsInRow - 1) * deerPositionParamters.SpacingHorizontal / 2f, 0f, (deerPositionParamters.RowCount - 1) * deerPositionParamters.SpacingVertical);
            targetPosition = position;
        }

        base.FixedUpdate();
        if (steerDirection != Vector3.zero)
        {
            animator.SetBool(Constants.AnimatorState.IsRunning, true);
        }
        else
        {
            animator.SetBool(Constants.AnimatorState.IsRunning, false);
        }
    }

    protected override bool ShouldMove()
    {
        return Utils.DistanceToTargetAboveThreshold(transform.position, targetPosition, targetReachedSquaredDistance);
    }
}
