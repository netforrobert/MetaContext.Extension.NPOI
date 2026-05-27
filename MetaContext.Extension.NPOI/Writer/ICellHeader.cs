using NPOI.SS.UserModel;

namespace MetaContext.Extension.NPOI.Writer;

public interface ICellHeader
{
    int Cols { get; }

    int Rows { get; }

    string HeaderText { get; }

    ICellHeader RightMerge(int cols);

    ICellHeader DownMerge(int rows);

    void SetHeaderText(string text, ICellStyle headerStyle = null);
}
