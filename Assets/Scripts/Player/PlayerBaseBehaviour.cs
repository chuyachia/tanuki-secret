using UnityEngine;

public class PlayerBaseBehaviour
{
    protected ModelController modelController;

    public PlayerBaseBehaviour(ModelController modelController)
    {
        this.modelController = modelController;
    }

    public virtual void HandleControllerColliderHit(ControllerColliderHit hit)
    {

    }

    public virtual void UpdateAnimatorBasedOnMovement(float velocity, Vector3 move, bool isGrounded)
    {
        modelController.Animator?.SetBool(Constants.AnimatorState.IsGrounded, isGrounded);
        if (move != Vector3.zero)
        {
            modelController.Animator?.SetBool(Constants.AnimatorState.IsWalking, true);
            modelController.Animator?.SetFloat(Constants.AnimatorState.speed, velocity);
        }
        else
        {
            modelController.Animator?.SetBool(Constants.AnimatorState.IsWalking, false);
        }
    }

    public virtual void Cleanup()
    {

    }

    public virtual bool ShouldMove()
    {
        return true;
    }

    public virtual bool CanRun()
    {
        return true;
    }
}