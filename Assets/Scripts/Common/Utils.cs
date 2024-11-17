
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

    public static void ActivateChildAndCopyMaterialFromTarget(Transform currentObject, GameObject target, int childIndex, string childTag)
    {
        if (currentObject.childCount < childIndex + 1)
        {
            return;
        }

        Transform childToActivate = currentObject.GetChild(childIndex);
        if (childToActivate.CompareTag(childTag))
        {
            Renderer childRenderer = childToActivate.gameObject.GetComponent<Renderer>();
            Renderer targetRenderer = target.GetComponent<Renderer>();
            if (childRenderer != null && targetRenderer != null)
            {
                childRenderer.material = targetRenderer.material;
            }
            childRenderer.gameObject.SetActive(true);
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
}
