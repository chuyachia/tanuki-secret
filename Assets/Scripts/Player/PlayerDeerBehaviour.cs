using UnityEngine;

public class PlayerDeerBehaviour : PlayerBaseBehaviour
{
    public PlayerDeerBehaviour(float runThreshold, Transform transform, ModelController modelController) : base(modelController)
    {
        this.runThreshold = runThreshold;
    }

    private float runThreshold;

    public void Attack()
    {
        modelController.Animator?.SetTrigger(Constants.AnimatorState.IsAttacking);
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