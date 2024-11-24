public class DanseCommand
{
    public DanseCommand(DanseMove danseMove, int beat)
    {
        Move = danseMove;
        Beat = beat;
    }

    public DanseMove Move
    {
        get;
    }

    public int Beat
    {
        get;
    }
}