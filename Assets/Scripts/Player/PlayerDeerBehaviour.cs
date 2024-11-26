using UnityEngine;

public class PlayerDeerBehaviour : PlayerBaseBehaviour
{
    public PlayerDeerBehaviour(Transform transform, ModelController modelController) : base(modelController)
    {
        this.transform = transform;
        EventManager.Instance.RegisterDeerLevelEventListener(HandleEvent);
    }

    private Transform transform;
    private bool inDeerGroup;

    public void HandleEvent(GameObject[] gameObjects, EventManager.DeerLevelEvent eventType)
    {
        switch (eventType)
        {
            case EventManager.DeerLevelEvent.StartJourney:
                {
                    inDeerGroup = true;
                    modelController.Animator?.SetBool(Constants.AnimatorState.IsWalking, false);
                    break;
                }
            case EventManager.DeerLevelEvent.ArriveAtDestination:
            case EventManager.DeerLevelEvent.PlayerTooFarFromDeers:
            case EventManager.DeerLevelEvent.NotEnoughDeersLeft:
                {
                    inDeerGroup = false;
                    modelController.Animator?.SetBool(Constants.AnimatorState.IsRunning, false);
                    break;
                }
        }
    }

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

    public override void UpdateAnimatorBasedOnMovement(Vector3 move, bool isGrounded)
    {
        string movementAnimateState = inDeerGroup ? Constants.AnimatorState.IsRunning : Constants.AnimatorState.IsWalking;
        modelController.Animator?.SetBool(Constants.AnimatorState.IsGrounded, isGrounded);
        if (move != Vector3.zero)
        {
            modelController.Animator?.SetBool(movementAnimateState, true);
        }
        else
        {
            modelController.Animator?.SetBool(movementAnimateState, false);
        }
    }
}