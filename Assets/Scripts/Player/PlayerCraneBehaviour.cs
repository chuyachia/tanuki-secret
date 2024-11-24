public class PlayerCraneBehaviour : PlayerBaseBehaviour
{
    private CraneDanseAnimator craneDanseAnimator;
    public PlayerCraneBehaviour(ModelController modelController) : base(modelController)
    {
        craneDanseAnimator = new CraneDanseAnimator(modelController.Model);
    }


    public void CraneDanse(float inputHorizontal, float inputVertical, bool inputSpace)
    {
        DanseMove danseMove = DanseMove.FromUserInput(inputHorizontal, inputVertical, inputSpace);
        craneDanseAnimator.Danse(danseMove);
    }

    public override bool ShouldMove()
    {
        return false;
    }
}