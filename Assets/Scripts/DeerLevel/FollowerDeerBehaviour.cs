using UnityEngine;

public class FollowerDeerBehaviour : TargetBasedSteerBehaviour
{

    [SerializeField] private float jitterAmount = 0.1f;
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

    protected override void Start()
    {
        base.Start();
        animator = GetComponentInChildren<Animator>();
    }

    protected override Vector3 GetTargetPosition()
    {
        if (target != null)
        {
            Vector3 targetPosition = target.transform.position - new Vector3(deerPositionParamters.NumberInRow * deerPositionParamters.SpacingHorizontal - (deerPositionParamters.ObjectsInRow - 1) * deerPositionParamters.SpacingHorizontal / 2f, 0f, (deerPositionParamters.RowCount - 1) * deerPositionParamters.SpacingVertical);
            return Utils.JitterPosition(targetPosition, jitterAmount);
        }
        else
        {
            return transform.position;
        }
    }

    protected override bool ShouldJump()
    {
        return false;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (steerDirection != Vector3.zero)
        {
            animator.SetBool(Constants.AnimatorState.IsWalking, true);
        }
        else
        {
            animator.SetBool(Constants.AnimatorState.IsWalking, false);
        }
    }

    protected override bool ShouldMove()
    {
        return base.ShouldMove() && Utils.DistanceToTargetAboveThreshold(transform.position, target.transform.position, targetReachedSquaredDistance);
    }
}
