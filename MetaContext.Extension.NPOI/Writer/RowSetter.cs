using System;

using MetaContext.Extension.NPOI.ColumnIndex;

using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace MetaContext.Extension.NPOI.Writer;

internal class RowSetter : IRowSetter
{
    private readonly IRow _row;
    private readonly ColumnIndices _colIndexs;

    public RowSetter(IRow row,
        ColumnIndices colIndexs,
        int rows = 1)
    {
        _row = row;
        _colIndexs = colIndexs;
        Rows = rows;
    }

    public int Rows { get; private set; }

    public ICellWriter CreaterCellWriter(string columnName, int index = 0, int cols = 1)
    {
        var colIndex = _colIndexs.GetColIndex(columnName, index)
            ?? throw new InvalidOperationException($"列名 '{columnName}' 不存在");
        return new CellWriter(_row, colIndex.StartIndex, Rows, cols);
    }

    public IRowSetter Set<TargetValue>(string columnName, TargetValue value, int index)
    {
        var colIndex = _colIndexs.GetColIndex(columnName, index) 
            ?? throw new InvalidOperationException($"列名 '{columnName}' 不存在");

        if (colIndex.EndIndex > colIndex.StartIndex)
        {
            //增加合并单元格
            CellRangeAddress region = new(_row.RowNum,
                _row.RowNum,
                colIndex.StartIndex,
                colIndex.EndIndex);
            _row.Sheet.AddMergedRegion(region);
        }

        var cell = _row.GetCell(colIndex.StartIndex) ?? _row.CreateCell(colIndex.StartIndex);
        cell.SetTargetValue(value);
        return this;
    }
}
