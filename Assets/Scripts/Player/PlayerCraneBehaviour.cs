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

    public override bool ShouldMove()
    {
        return !isDanceMode;
    }
}