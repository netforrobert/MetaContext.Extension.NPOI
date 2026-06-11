namespace MetaContext.Extension.NPOI.Header;

public interface IHeaderInfo
{
    int RowIndex { get; }

    int ColumnIndex { get; }

    int Rows { get; }

    int Columns { get; }

    string HeaderText { get; }
}
