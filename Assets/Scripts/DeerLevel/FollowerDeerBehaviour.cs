using UnityEngine;

public class FollowerDeerBehaviour : TargetBasedSteerBehaviour
{

    [SerializeField] private float jitterAmount = 0.1f;
    public GameObject Leader
    {
        set
        {
            target = value;
        }
    }

    public DeerPositionParamters deerPositionParamters;

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
}
