public readonly struct DeerPositionParamters
{
    public DeerPositionParamters(int rowCount, int numberInRow, int objectsInRow, float spacingHorizontal, float spacingVertical)
    {
        RowCount = rowCount;
        NumberInRow = numberInRow;
        ObjectsInRow = objectsInRow;
        SpacingHorizontal = spacingHorizontal;
        SpacingVertical = spacingVertical;
    }

    public int RowCount { get; }
    public int NumberInRow { get; }
    public int ObjectsInRow { get; }
    public float SpacingHorizontal { get; }
    public float SpacingVertical { get; }

}