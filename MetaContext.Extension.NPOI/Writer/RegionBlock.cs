using System;

using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;

namespace MetaContext.Extension.NPOI.Writer;

internal class RegionBlock : IRegionBlock
{
    private readonly ISheet _sheet;
    private readonly IRow _row;
    private readonly ICellStyle _cellStyle;
    private readonly int _startColumnIndex;
    private int _columnIndex;

    public RegionBlock(ISheet sheet,
        ICellStyle cellStyle,
        int startRowIndex, 
        int startColumnIndex)
    {
        _sheet = sheet;
        _cellStyle = cellStyle;
        _row = _sheet.GetRow(startRowIndex) ?? _sheet.CreateRow(startRowIndex);
        _startColumnIndex = startColumnIndex;
        _columnIndex = startColumnIndex;
        Rows = 1;
        Cols = 1;
    }

    public int StartRowIndex
        => _row.RowNum;

    public int StartColIndex
        => _startColumnIndex;

    public int Rows { get; private set; }

    public int Cols { get; private set; }

    public void Block(string text, Action<IRegionBlock> action)
    {
        var block = new RegionBlock(_sheet,
            _cellStyle,
            _row.RowNum + Rows,
            _columnIndex);
        action(block);
        var regionCell = new RegionCell(_row, _columnIndex);
        regionCell.SetValue(text, rightMerge: block.Cols);
    }

    public void Col(string text, int rightMerge = 1, int downMerge = 1)
    {
        _columnIndex += (Cols - 1);
        var regionCell = new RegionCell(_row, _columnIndex);
        regionCell.SetValue(text, rightMerge, downMerge, _cellStyle);
        Rows += downMerge;
        Cols += rightMerge;
    }
}
