namespace MetaContext.Extension.NPOI.Reader;

public record ColKey
{
    public ColKey(string column,
        int index)
    {
        Column = column;
        Index = index;
    }

    public string Column { get; private set; }
    public int Index { get; private set; }
}
