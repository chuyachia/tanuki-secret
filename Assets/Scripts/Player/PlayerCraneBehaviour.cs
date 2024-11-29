using UnityEngine;

public class PlayerCraneBehaviour : PlayerBaseBehaviour
{
    private bool isDanceMode;
    private Transform transform;

    public PlayerCraneBehaviour(Transform transform, ModelController modelController) : base(modelController)
    {
        this.transform = transform;
        EventManager.Instance.RegisterCraneLevelEventListener(HandleEvent);
    }

    public void HandleEvent(GameObject[] gameObjects, EventManager.CraneLevelEvent eventType)
    {
        if (eventType == EventManager.CraneLevelEvent.StartDance)
        {
            modelController.BodyAnimator?.SetBool(Constants.AnimatorState.IsWalking, false);
            modelController.WingAnimator?.SetBool(Constants.AnimatorState.IsWalking, false);
            isDanceMode = true;
            if (transform.localScale.x < 0)
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
        }
    }

    public void CraneDanse(float inputHorizontal, float inputVertical, bool inputSpace)
    {
        DanceMove danseMove = DanceMove.FromUserInput(inputHorizontal, inputVertical, inputSpace);
        modelController.Dance(danseMove);
    }


    public override void UpdateAnimatorBasedOnMovement(float velocity, Vector3 move, bool isGrounded)
    {
        if (move != Vector3.zero)
        {
            modelController.BodyAnimator?.SetBool(Constants.AnimatorState.IsWalking, true);
            modelController.WingAnimator?.SetBool(Constants.AnimatorState.IsWalking, true);
        }
        else
        {
            modelController.BodyAnimator?.SetBool(Constants.AnimatorState.IsWalking, false);
            modelController.WingAnimator?.SetBool(Constants.AnimatorState.IsWalking, false);
        }
    }

    public override bool ShouldMove()
    {
        return !isDanceMode;
    }

    public override bool CanRun()
    {
        return false;
    }
}