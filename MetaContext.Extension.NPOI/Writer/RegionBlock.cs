using System;
using System.Collections.Generic;
using System.Linq;

using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;

namespace MetaContext.Extension.NPOI.Writer;

internal class RegionBlock : IRegionBlock
{
    private readonly List<IRegionBlock> _blocks = new();
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
            _row.RowNum + 1,
            _columnIndex);
        action(block);
        _blocks.Add(block);
        var regionCell = new RegionCell(_row, _columnIndex);
        regionCell.SetValue(text, rightMerge: block.Cols, cellStyle: _cellStyle);
        Cols += block.Cols;
        Rows = _blocks.Select(p => p.Rows).Max() + 1;
        _columnIndex += block.Cols;
    }

    public void Col(string text, int rightMerge = 1, int downMerge = 1)
    {
        var regionCell = new RegionCell(_row, _columnIndex);
        regionCell.SetValue(text, rightMerge, downMerge, _cellStyle);
        _columnIndex += rightMerge;
        Cols += rightMerge;
        Rows = Math.Max(Rows, downMerge);
    }
}
