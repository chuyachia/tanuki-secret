using UnityEngine;

public class PlayerCraneBehaviour : PlayerBaseBehaviour
{
    private CraneDanceAnimator craneDanseAnimator;
    private bool isDanceMode;
    private Transform transform;

    public PlayerCraneBehaviour(Transform transform, ModelController modelController) : base(modelController)
    {
        craneDanseAnimator = new CraneDanceAnimator(modelController.Model);
        this.transform = transform;
        EventManager.Instance.RegisterCraneLevelEventListener(HandleEvent);
    }

    public void HandleEvent(GameObject[] gameObjects, EventManager.CraneLevelEvent eventType)
    {
        if (eventType == EventManager.CraneLevelEvent.StartDance)
        {
            Debug.Log("Start dance");
            craneDanseAnimator.BodyAnimator?.SetBool(Constants.AnimatorState.IsWalking, false);
            craneDanseAnimator.WingAnimator?.SetBool(Constants.AnimatorState.IsWalking, false);
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
        craneDanseAnimator.Danse(danseMove);
    }


    public override void UpdateAnimatorBasedOnMovement(float velocity, Vector3 move, bool isGrounded)
    {
        if (move != Vector3.zero)
        {
            craneDanseAnimator.BodyAnimator?.SetBool(Constants.AnimatorState.IsWalking, true);
            craneDanseAnimator.WingAnimator?.SetBool(Constants.AnimatorState.IsWalking, true);
        }
        else
        {
            craneDanseAnimator.BodyAnimator?.SetBool(Constants.AnimatorState.IsWalking, false);
            craneDanseAnimator.WingAnimator?.SetBool(Constants.AnimatorState.IsWalking, false);
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