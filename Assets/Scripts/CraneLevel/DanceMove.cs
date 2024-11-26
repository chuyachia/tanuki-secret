public struct DanceMove
{
    public static DanceMove NoMove = new DanceMove(WingPosition.ROLLED, BodyPosition.NEUTRAL);
    public WingPosition WingPosition { get; }
    public BodyPosition BodyPosition { get; }

    public DanceMove(WingPosition wingPosition, BodyPosition bodyPosition)
    {
        this.WingPosition = wingPosition;
        this.BodyPosition = bodyPosition;
    }

    public static DanceMove FromUserInput(float inputHorizontal, float inputVertical, bool inputSpace)
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
            bodyPosition = BodyPosition.RIGHT;
        }
        else if (inputHorizontal < 0)
        {
            bodyPosition = BodyPosition.LEFT;
        }
        return new DanceMove(wingPosition, bodyPosition);
    }
}