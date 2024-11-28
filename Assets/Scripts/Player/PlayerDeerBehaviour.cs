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
            case EventManager.DeerLevelEvent.PlayerAttackWolf:
                {
                    modelController.Animator?.SetTrigger(Constants.AnimatorState.IsAttacking);
                    break;
                }
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