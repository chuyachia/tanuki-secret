
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static Vector3 StripYDimension(Vector3 v3)
    {
        return new Vector3(v3.x, 0, v3.z);
    }

    public static bool DistanceToTargetWithinThreshold(Vector3 source, Vector3 target, float threshold)
    {
        return (Utils.StripYDimension(target) - Utils.StripYDimension(source)).sqrMagnitude <= threshold;
    }

    public static bool DistanceToTargetAboveThreshold(Vector3 source, Vector3 target, float threshold)
    {
        return (Utils.StripYDimension(target) - Utils.StripYDimension(source)).sqrMagnitude > threshold;
    }

    public static Quaternion RandomRotationRoundY()
    {
        float randomYRotation = Random.Range(0f, 360f);
        return Quaternion.Euler(0f, randomYRotation, 0f);
    }

    public static Vector3 JitterPosition(Vector3 originalPosition, float jitterAmount)
    {
        float jitterX = Random.Range(-jitterAmount, jitterAmount);
        float jitterZ = Random.Range(-jitterAmount, jitterAmount);
        return originalPosition + new Vector3(jitterX, 0f, jitterZ);
    }

    public static void ActivateChild(Transform currentObject, int childIndex, string childTag)
    {
        if (currentObject.childCount < childIndex + 1)
        {
            return;
        }

        Transform childToActivate = currentObject.GetChild(childIndex);
        if (childToActivate.CompareTag(childTag))
        {
            childToActivate.gameObject.SetActive(true);
        }
    }

    public static void DeactivteChild(Transform currentObject, int childIndex, string childTag)
    {
        if (currentObject.childCount < childIndex + 1)
        {
            return;
        }

        Transform nutInMouth = currentObject.GetChild(childIndex);
        if (nutInMouth.CompareTag(childTag))
        {
            nutInMouth.gameObject.SetActive(false);
        }
    }

    public static List<Vector3> GetCircleAroundTargetPosition(int numberOfObjectsToPlace, Vector3 center, float startAngle, float spanAngle, float radius)
    {
        List<Vector3> result = new List<Vector3>();
        float angleStep = spanAngle / (numberOfObjectsToPlace - 1);
        float angleOffset = startAngle - spanAngle / 2f;
        for (int i = 0; i < numberOfObjectsToPlace; i++)
        {
            float angleInDegrees = angleOffset + (i * angleStep);
            float angleInRadians = Mathf.Deg2Rad * angleInDegrees;

            float x = center.x + Mathf.Cos(angleInRadians) * radius;
            float z = center.z + Mathf.Sin(angleInRadians) * radius;

            result.Add(new Vector3(x, center.y, z));
        }
        return result;
    }
}
