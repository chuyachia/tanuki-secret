

using System.Collections.Generic;
using UnityEngine;

public class LeaderDeerBehaviour : TargetBasedSteerBehaviour
{
    [SerializeField] private float targetReachedSquaredDistance = 2f;
    private List<GameObject> targets = new List<GameObject>();
    private int currentTarget = 0;

    public void StartMove(List<GameObject> waypoints)
    {
        targets.AddRange(waypoints);
    }

    public void ResetMoveState()
    {
        targets.Clear();
        currentTarget = 0;
    }

    protected override bool ShouldJump()
    {
        return false;
    }

    void Update()
    {
        if (currentTarget == targets.Count)
        {
            target = null;
        }
        else
        {
            target = targets[currentTarget];
            if (Utils.DistanceToTargetWithinThreshold(transform.position, targets[currentTarget].transform.position, targetReachedSquaredDistance))
            {
                currentTarget++;
            }
        }

    }
}
