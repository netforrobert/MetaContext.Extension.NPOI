using System;

using MetaContext.Extension.NPOI.ColumIndex;

using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace MetaContext.Extension.NPOI.Writer;

internal class RowSetter : IRowSetter
{
    private readonly IRow _row;
    private readonly ColumnIndices _colIndexs;

    public RowSetter(IRow row, ColumnIndices colIndexs)
    {
        _row = row;
        _colIndexs = colIndexs;
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
        var type = typeof(TargetValue);
        switch (type)
        {
            case Type _ when value == null:
                break;
            case Type t when t == typeof(bool):
                cell.SetCellValue((bool)(object)value);
                break;
            case Type t when t == typeof(int):
                cell.SetCellValue((int)(object)value);
                break;
            case Type t when t == typeof(long):
                cell.SetCellValue((long)(object)value);
                break;
            case Type t when t == typeof(DateTime):
                cell.SetCellValue((DateTime)(object)value);
                break;
            case Type t when t == typeof(decimal) || t == typeof(double):
                cell.SetCellValue((double)(object)value);
                break;
            case Type t when t == typeof(string):
            default:
                cell.SetCellValue(value.ToString());
                break;
        }

        return this;
    }
}
