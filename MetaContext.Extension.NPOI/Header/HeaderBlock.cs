using System;
using System.Collections.Generic;
using System.Linq;

using MetaContext.Extension.NPOI.ColumnIndex;

using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;

namespace MetaContext.Extension.NPOI.Header;

internal class HeaderBlock : IHeaderBlock
{
    private readonly List<IHeaderBlock> _blocks = new();
    private readonly List<IHeaderCell> _cells = new();
    private readonly ISheet _sheet;
    private readonly ICellStyle _cellStyle;
    private readonly string _titleText;

    public HeaderBlock(ISheet sheet,
        ICellStyle cellStyle,
        int rowIndex,
        int columnIndex,
        string titleText)
    {
        _sheet = sheet;
        _cellStyle = cellStyle;
        RowIndex = rowIndex;
        ColumnIndex = columnIndex;
        _titleText = titleText;
    }

    public int RowIndex { get; private set; }

    public int ColumnIndex { get; private set; }

    public int Rows
    {
        get
        {
            int cellsRows = _cells.Count switch
            {
                0 => 1,
                _ => _cells.Select(p => p.Rows).Max()
            };

            int blocksRows = _blocks.Count switch
            {
                0 => 1,
                _ => _blocks.Select(p => p.Rows).Max()
            };

            var rows = Math.Max(cellsRows, blocksRows)
                + (_titleText == null ? 0 : 1);
            return rows;
        }
    }

    public int Columns
    {
        get 
        {
            int cellsCols = _cells.Count switch
            {
                0 => 0,
                _ => _cells.Select(p => p.Columns).Sum()
            };

            int blocksCols = _blocks.Count switch
            {
                0 => 0,
                _ => _blocks.Select(p => p.Columns).Sum()
            };

            return Math.Max(cellsCols, blocksCols);
        }
    }

    public IHeaderCell Title { get; private set; }

    public ICollection<IHeaderCell> Cells => _cells.AsReadOnly();

    public ICollection<IHeaderBlock> Blocks => _blocks.AsReadOnly();

    public void Block(string text, 
        Action<IHeaderBlock> action,
        ICellStyle cellStyle = null)
    {
        int colIndex = ColumnIndex + Columns;
        var block = new HeaderBlock(_sheet,
            _cellStyle,
            RowIndex + 1,
            colIndex,
            text);
        action(block);
        block.SetTitle(cellStyle);
        _blocks.Add(block);
    }

    public void Cell(string text, 
        int rightMerge = 1, 
        int downMerge = 1,
        ICellStyle headerStyle = null)
    {
        int rowIndex = _titleText switch
        {
            null => RowIndex,
            _ => RowIndex + 1
        };

        IRow row = _sheet.GetRow(rowIndex) ?? _sheet.CreateRow(rowIndex);
        int colIndex = ColumnIndex + Columns;
        var headerCell = new HeaderCell(row, colIndex);
        headerStyle ??= _cellStyle;
        headerCell.Text(text, rightMerge, downMerge, headerStyle);
        _cells.Add(headerCell);
    }

    public void SetTitle(ICellStyle headerStyle = null)
    {
        if (_titleText == null)
            return;

        var row = _sheet.GetRow(RowIndex) ?? _sheet.CreateRow(RowIndex);
        var headerCell = new HeaderCell(row, ColumnIndex);
        headerStyle ??= _cellStyle;
        headerCell.Text(_titleText, rightMerge: Columns, cellStyle: headerStyle);
        Title = headerCell;
    }
}
