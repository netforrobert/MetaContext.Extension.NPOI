namespace MetaContext.Extension.NPOI.Header;

public record HeaderInfo
{
    public HeaderInfo(HeaderInfo header)
    {
        RowIndex = header.RowIndex;
        ColumnIndex = header.ColumnIndex;
        Rows = header.Rows;
        Columns = header.Columns;
        HeaderText = header.HeaderText;
    }

    public HeaderInfo(IHeaderCell headerCell)
    {
        RowIndex = headerCell.RowIndex;
        ColumnIndex = headerCell.ColumnIndex;
        Rows = headerCell.Rows;
        Columns = headerCell.Columns;
        HeaderText = headerCell.HeaderText;
    }

    public int RowIndex { get; private set; }

    public int ColumnIndex { get; private set; }

    public int Rows { get; private set; }

    public int Columns { get; private set; }

    public string HeaderText { get; private set; }

    public override string ToString()
        => $"{HeaderText}({RowIndex},{Rows},{ColumnIndex},{Columns})";
}
