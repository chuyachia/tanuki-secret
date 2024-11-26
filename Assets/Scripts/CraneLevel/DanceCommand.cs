public struct DanceCommand
{
    public DanceCommand(DanceMove danseMove, int beat)
    {
        Move = danseMove;
        Beat = beat;
    }

    public DanceMove Move
    {
        get;
    }

    public int Beat
    {
        get;
    }
}