using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace MetaContext.Extension.NPOI.Writer;

internal class CellWriter : ICellWriter
{
    private readonly IRow _row;
    private CellRangeAddress _region;
    private ICellStyle _cellStyle;
    private readonly int _maxRows;
    private readonly int _maxCols;

    public CellWriter(IRow row,
        int columnIndex,
        int maxRows,
        int maxCols)
    {
        _row = row;
        RowIndex = row.RowNum;
        ColumnIndex = columnIndex;
        _maxRows = maxRows;
        _maxCols = maxCols;
        Rows = maxRows;
        Cols = maxCols;
    }

    public int RowIndex { get; private set; }

    public int ColumnIndex { get; private set; }

    public int Cols { get; private set; }

    public int Rows { get; private set; }

    public ICellWriter DownMerge(int rows)
    {
        Rows = rows switch
        {
            < 1 => 1,
            > 1 when rows > _maxRows => _maxRows,
            _ => rows,
        };

        return this;
    }

    public ICellWriter RightMerge(int cols)
    {
        Cols = cols switch
        {
            < 1 => 1,
            > 1 when cols > _maxCols => _maxCols,
            _ => cols,
        };

        return this;
    }

    public ICellWriter SetStyle(ICellStyle cellStyle)
    {
        _cellStyle = cellStyle;
        return this;
    }

    public void SetValue<T>(T value)
    {
        var sheet = _row.Sheet;
        switch (Rows, Cols)
        {
            case (1, 1):
                break;
            default:
                //增加合并单元格
                _region = new(RowIndex,
                    RowIndex + (Rows - 1),
                    ColumnIndex,
                    ColumnIndex + (Cols - 1));
                sheet.AddMergedRegion(_region);
                break;
        }

        var cell = _row.GetCell(ColumnIndex) ?? _row.CreateCell(ColumnIndex);
        cell.SetTargetValue(value);
        if (_cellStyle != null)
        {
            cell.CellStyle = _cellStyle;
            _region?.SetNoramlBorder(_cellStyle, sheet);
        }
    }
}
