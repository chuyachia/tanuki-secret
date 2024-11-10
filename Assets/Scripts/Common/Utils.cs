
using UnityEngine;

public static class Utils
{
    public static Vector3 StripYDimension(Vector3 v3)
    {
        return new Vector3(v3.x, 0, v3.z);
    }

    public static Quaternion RandomRotationRoundY()
    {
        float randomYRotation = Random.Range(0f, 360f);
        return Quaternion.Euler(0f, randomYRotation, 0f);
    }
}
