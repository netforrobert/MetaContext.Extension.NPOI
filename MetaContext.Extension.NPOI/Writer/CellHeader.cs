using System;

using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace MetaContext.Extension.NPOI.Writer;

internal class CellHeader : ICellHeader
{
    private readonly int _startColIndex;
    private readonly int _startRowIndex;
    private readonly int _maxRows;
    private readonly ISheet _sheet;
    private readonly ICellStyle _defaultHeaderStyle;

    private int _cols = 1;
    private int _rows = 1;

    public CellHeader(int startColIndex,
        int startRowIndex,
        int maxRows,
        ISheet sheet,
        ICellStyle defaultHeaderStyle)
    {
        _startColIndex = startColIndex;
        _startRowIndex = startRowIndex;
        _maxRows = maxRows;
        _sheet = sheet;
        _defaultHeaderStyle = defaultHeaderStyle;
    }

    public int Cols => _cols;

    public string HeaderText { get; private set; }

    public ICellHeader DownMerge(int rows)
    {
        if (rows > _maxRows)
            throw new NotSupportedException($"不能超过最大行数：{_maxRows}");

        _rows = rows;
        if (_rows < 1)
            _rows = 1;

        return this;
    }

    public ICellHeader RightMerge(int cols)
    {
        _cols = cols;
        if (_cols < 1)
            _cols = 1;

        return this;
    }

    public void SetHeaderText(string text, ICellStyle headerStyle)
    {
        headerStyle ??= _defaultHeaderStyle;
        switch (_rows, _cols)
        {
            case (1, 1):
                break;
            default:
                //增加合并单元格
                CellRangeAddress region = new(_startRowIndex,
                    _startRowIndex + (_rows - 1),
                    _startColIndex,
                    _startColIndex + (_cols -1));
                _sheet.AddMergedRegion(region);
                region.SetNoramlBorder(headerStyle, _sheet);
                break;
        }

        var row = _sheet.GetRow(_startRowIndex) ?? _sheet.CreateRow(_startRowIndex);
        var cell = row.GetCell(_startColIndex) ?? row.CreateCell(_startColIndex);
        cell.SetCellValue(text);
        cell.CellStyle = headerStyle;
        HeaderText = text;
    }
}
