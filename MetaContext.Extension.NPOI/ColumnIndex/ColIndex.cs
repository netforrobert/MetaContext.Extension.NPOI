namespace MetaContext.Extension.NPOI.ColumnIndex;

public record ColIndex
{
    public ColIndex(int startIndex,
        int endIndex,
        string name,
        int relativeIndex)
    {
        StartIndex = startIndex;
        EndIndex = endIndex;
        Name = name;
        RelativeIndex = relativeIndex;
    }

    public int StartIndex { get; private set; }

    public int EndIndex { get; private set; }

    public string Name { get; private set; }

    public int RelativeIndex { get; private set; }
}