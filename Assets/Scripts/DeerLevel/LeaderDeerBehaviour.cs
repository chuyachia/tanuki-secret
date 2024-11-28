using System.Collections.Generic;
using UnityEngine;

public class LeaderDeerBehaviour : DeerBehaviour
{
    private List<GameObject> targets = new List<GameObject>();
    private int currentTarget = 0;

    public void StartMove(List<GameObject> waypoints)
    {
        targets.AddRange(waypoints);
    }


    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (targetReached && state == State.Migrating)
        {
            currentTarget++;
            if (currentTarget == targets.Count)
            {
                EventManager.Instance.InvokeDeerLevelEvent(new GameObject[] { }, EventManager.DeerLevelEvent.ArriveAtDestination);
            }
        }
    }

    protected override Vector3 GetMigrationTarget()
    {
        if (currentTarget == targets.Count)
        {
            return transform.position;
        }
        else
        {
            return targetPosition = targets[currentTarget].transform.position;
        }
    }
}
