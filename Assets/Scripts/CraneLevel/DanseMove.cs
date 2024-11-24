public class DanseMove
{
    public static DanseMove NoMove = new DanseMove(WingPosition.ROLLED, BodyPosition.NEUTRAL);
    public WingPosition WingPosition { get; }
    public BodyPosition BodyPosition { get; }

    public DanseMove(WingPosition wingPosition, BodyPosition bodyPosition)
    {
        this.WingPosition = wingPosition;
        this.BodyPosition = bodyPosition;
    }

    public static DanseMove FromUserInput(float inputHorizontal, float inputVertical, bool inputSpace)
    {
        WingPosition wingPosition = inputSpace ? WingPosition.DEPLOYED : WingPosition.ROLLED;
        BodyPosition bodyPosition = BodyPosition.NEUTRAL;
        if (inputVertical > 0)
        {
            bodyPosition = BodyPosition.UP;
        }
        else if (inputVertical < 0)
        {
            bodyPosition = BodyPosition.DOWN;
        }
        else if (inputHorizontal > 0)
        {
            bodyPosition = BodyPosition.SIDE_RIGHT;
        }
        else if (inputHorizontal < 0)
        {
            bodyPosition = BodyPosition.SIDE_LEFT;
        }
        return new DanseMove(wingPosition, bodyPosition);
    }
}