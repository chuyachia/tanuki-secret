using UnityEngine;

public class PlayerDeerBehaviour : PlayerBaseBehaviour
{
    public PlayerDeerBehaviour(float runThreshold, Transform transform, ModelController modelController) : base(modelController)
    {
        this.runThreshold = runThreshold;
        EventManager.Instance.RegisterDeerLevelEventListener(HandleEvent);
    }

    private float runThreshold;
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

    public override void UpdateAnimatorBasedOnMovement(float velocity, Vector3 move, bool isGrounded)
    {
        modelController.Animator?.SetBool(Constants.AnimatorState.IsGrounded, isGrounded);
        if (move != Vector3.zero)
        {
            modelController.Animator?.SetBool(Constants.AnimatorState.IsWalking, true);
            if (velocity > runThreshold)
            {
                modelController.Animator?.SetBool(Constants.AnimatorState.IsRunning, true);
            }
            else
            {
                modelController.Animator?.SetBool(Constants.AnimatorState.IsRunning, false);
            }
        }
        else
        {
            modelController.Animator?.SetBool(Constants.AnimatorState.IsWalking, false);
            modelController.Animator?.SetBool(Constants.AnimatorState.IsRunning, false);
        }
    }
}