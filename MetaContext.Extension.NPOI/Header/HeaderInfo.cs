namespace MetaContext.Extension.NPOI.Header;

internal class HeaderInfo : IHeaderInfo
{
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
}
