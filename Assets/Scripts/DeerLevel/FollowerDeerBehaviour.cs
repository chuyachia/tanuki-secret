using UnityEngine;

public class FollowerDeerBehaviour : DeerBehaviour
{
    public GameObject Target
    {
        set
        {
            target = value;
        }
    }

    public DeerPositionParamters deerPositionParamters;
    private GameObject target;

    public void CaughtByWolf()
    {
        target = null;
        animator.SetBool(Constants.AnimatorState.IsAttacked, true);
    }

    protected override Vector3 GetMigrationTarget()
    {
        if (target == null)
        {
            return transform.position;
        }
        return target.transform.position - new Vector3(deerPositionParamters.NumberInRow * deerPositionParamters.SpacingHorizontal - (deerPositionParamters.ObjectsInRow - 1) * deerPositionParamters.SpacingHorizontal / 2f, 0f, (deerPositionParamters.RowCount - 1) * deerPositionParamters.SpacingVertical);
    }
}
