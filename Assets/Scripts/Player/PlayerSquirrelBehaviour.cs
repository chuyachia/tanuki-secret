using UnityEngine;

public class PlayerSquirrelBehaviour : PlayerLevelBehaviour
{
    private bool hasNut;
    private Transform transform;
    private GameObject nutInMouth;
    public PlayerSquirrelBehaviour(Transform transform, ModelController modelController) : base(modelController)
    {
        this.transform = transform;
    }

    public override void HandleControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag(Constants.Tags.Nut) && hit.gameObject.activeSelf && !hasNut)
        {
            nutInMouth = Object.Instantiate(hit.gameObject, transform.position, Quaternion.identity);
            nutInMouth.transform.parent = transform;
            EventManager.Instance.InvokeSquirrelLevelEvent(new GameObject[] { hit.gameObject }, EventManager.SquirelLevelEvent.PickUpNut);
            hasNut = true;
            return;
        }
        if (hit.gameObject.CompareTag(Constants.Tags.NutBucket) && hasNut)
        {
            Object.Destroy(nutInMouth);
            EventManager.Instance.InvokeSquirrelLevelEvent(new GameObject[] { hit.gameObject }, EventManager.SquirelLevelEvent.PutNutInBucket);
            hasNut = false;
            return;
        }
    }

    public override void Cleanup()
    {
        Object.Destroy(nutInMouth);
    }
}