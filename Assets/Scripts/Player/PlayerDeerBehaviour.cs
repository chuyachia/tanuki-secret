using UnityEngine;

public class PlayerDeerBehaviour : PlayerBaseBehaviour
{
    public PlayerDeerBehaviour(Transform transform, ModelController modelController) : base(modelController)
    {
        this.transform = transform;
    }

    private Transform transform;

    public override void HandleControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag(Constants.Tags.Wolf) && hit.gameObject.activeSelf)
        {
            modelController.Animator?.SetTrigger(Constants.AnimatorState.IsAttacking);
            EventManager.Instance.InvokeDeerLevelEvent(new GameObject[] { hit.gameObject }, EventManager.DeerLevelEvent.WolfFlee);
            Vector3 directionAwayFromCollider = hit.gameObject.transform.position - transform.position;
            directionAwayFromCollider.Normalize();
            hit.gameObject.GetComponent<WolfBehaviour>().FleeTarget = directionAwayFromCollider * 20f;
        }
    }
}