using NPOI.SS.UserModel;

namespace MetaContext.Extension.NPOI.Writer;

public interface ICellWriter
{
    int RowIndex { get; }

    int ColumnIndex { get; }

    int Cols { get; }

    int Rows { get; }

    ICellWriter RightMerge(int cols);

    ICellWriter DownMerge(int rows);

    ICellWriter SetStyle(ICellStyle cellStyle);

    void SetValue<T>(T value);
}
