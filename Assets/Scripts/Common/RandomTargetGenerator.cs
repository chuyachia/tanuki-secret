using UnityEngine;

public class WanderTargetGenerator
{
    private float wanderRadius;
    private Vector3 wanderCenter;

    public WanderTargetGenerator(float wanderRadius, Vector3 wanderCenter)
    {
        this.wanderRadius = wanderRadius;
        this.wanderCenter = wanderCenter;
    }

    public Vector3 GetNewTarget()
    {
        Vector2 randomDirection = Random.insideUnitCircle * wanderRadius;
        return new Vector3(randomDirection.x + wanderCenter.x, 0, randomDirection.y + wanderCenter.z);
    }
}