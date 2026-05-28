using MetaContext.Extension.NPOI.ColumnIndex;

using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace MetaContext.Extension.NPOI.Writer;

internal class RegionCell : IRegionCell
{
    private readonly IRow _row;
    private readonly int _colIndex;

    public RegionCell(IRow row, int colIndex)
    {
        _row = row;
        _colIndex = colIndex;
    }

    public void SetValue<T>(T value, int rightMerge = 1, int downMerge = 1, ICellStyle cellStyle = null)
    {
        var sheet = _row.Sheet;
        int rowIndex = _row.RowNum;
        CellRangeAddress region = null;
        switch (rightMerge, downMerge)
        {
            case (1, 1):
                break;
            default:
                //增加合并单元格
                region = new(rowIndex,
                    rowIndex + (downMerge - 1),
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
