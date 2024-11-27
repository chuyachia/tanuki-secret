public struct DanceMove
{
    public static DanceMove NoMove = new DanceMove(WingPosition.NEUTRAL, BodyPosition.NEUTRAL);
    public static DanceMove Invalid = new DanceMove(WingPosition.INVALID, BodyPosition.INVALID);
    public WingPosition WingPosition { get; }
    public BodyPosition BodyPosition { get; }

    public DanceMove(WingPosition wingPosition, BodyPosition bodyPosition)
    {
        this.WingPosition = wingPosition;
        this.BodyPosition = bodyPosition;
    }

    public static DanceMove FromUserInput(float inputHorizontal, float inputVertical, bool inputSpace)
    {
        WingPosition wingPosition = inputSpace ? WingPosition.DEPLOYED : WingPosition.NEUTRAL;
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

    public DanceMove merge(DanceMove other)
    {
        if (this.Equals(other))
        {
            return this;
        }
        WingPosition mergedWingPosition;
        if (this.WingPosition.Equals(other.WingPosition))
        {
            mergedWingPosition = this.WingPosition;
        }
        else if (this.WingPosition.Equals(WingPosition.NEUTRAL) && !other.WingPosition.Equals(WingPosition.NEUTRAL))
        {
            mergedWingPosition = other.WingPosition;
        }
        else if (!this.WingPosition.Equals(WingPosition.NEUTRAL) && other.WingPosition.Equals(WingPosition.NEUTRAL))
        {
            mergedWingPosition = this.WingPosition;
        }
        else
        {
            return Invalid;
        }

        BodyPosition mergeBodyPosition;
        if (this.BodyPosition.Equals(other.BodyPosition))
        {
            mergeBodyPosition = this.BodyPosition;
        }
        else if (this.BodyPosition.Equals(BodyPosition.NEUTRAL) && !other.BodyPosition.Equals(BodyPosition.NEUTRAL))
        {
            mergeBodyPosition = other.BodyPosition;
        }
        else if (!this.BodyPosition.Equals(BodyPosition.NEUTRAL) && other.BodyPosition.Equals(BodyPosition.NEUTRAL))
        {
            mergeBodyPosition = this.BodyPosition;
        }
        else
        {
            return Invalid;
        }
        return new DanceMove(mergedWingPosition, mergeBodyPosition);
    }
}