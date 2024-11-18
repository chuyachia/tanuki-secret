

using System.Collections.Generic;
using UnityEngine;

public class LeaderDeerBehaviour : TargetBasedSteerBehaviour
{
    [SerializeField] private float targetReachedSquaredDistance;
    private List<GameObject> waypoints = new List<GameObject>();
    private int currentWaypoint = 0;

    public void StartJourney(List<GameObject> waypoints)
    {
        this.waypoints.AddRange(waypoints);
    }

    protected override bool ShouldJump()
    {
        return false;
    }

    void Update()
    {
        if (currentWaypoint == waypoints.Count)
        {
            target = null;
        }
        else
        {
            target = waypoints[currentWaypoint];
            if ((Utils.StripYDimension(waypoints[currentWaypoint].transform.position) - Utils.StripYDimension(transform.position)).sqrMagnitude < targetReachedSquaredDistance)
            {
                currentWaypoint++;
            }
        }

    }
}
