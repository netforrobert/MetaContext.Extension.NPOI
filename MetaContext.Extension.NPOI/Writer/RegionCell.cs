using System;

using NPOI.SS.UserModel;
using NPOI.SS.Util;


namespace MetaContext.Extension.NPOI.Writer;

internal class RegionCell : IRegionCell
{
    private readonly IRow _row;
    private readonly int _colIndex;
    private readonly int _rowIndex;

    public RegionCell(IRow row, int colIndex)
    {
        _row = row;
        _rowIndex = row.RowNum;
        _colIndex = colIndex;
        Rows = 1;
        Columns = 1;
    }

    public int RowIndex
        => _rowIndex;

    public int ColumnIndex
        => _colIndex;

    public int Rows { get; private set; }

    public int Columns { get; private set; }

    public void SetValue<T>(T value, int rightMerge = 1, int downMerge = 1, ICellStyle cellStyle = null)
    {
        if (rightMerge < 1)
            throw new ArgumentException("rightMerge不能小于1");

        if (downMerge < 1)
            throw new ArgumentException("downMerge不能小于1");

        Rows = downMerge; 
        Columns = downMerge;

        var sheet = _row.Sheet;
        CellRangeAddress region = null;
        switch (rightMerge, downMerge)
        {
            case (1, 1):
                break;
            default:
                //增加合并单元格
                region = new(_rowIndex,
                    _rowIndex + (downMerge - 1),
                    _colIndex,
                    _colIndex + (rightMerge - 1));
                sheet.AddMergedRegion(region);
                break;
        }

        var cell = _row.GetCell(_colIndex) ?? _row.CreateCell(_colIndex);
        cell.SetTargetValue(value);
        if (cellStyle != null)
        {
            cell.CellStyle = cellStyle;
            region?.SetNoramlBorder(cellStyle, sheet);
        }
    }
}
