using System;
using System.Collections.Generic;
using System.Linq;

using MetaContext.Extension.NPOI.ColumnIndex;

using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;

namespace MetaContext.Extension.NPOI.Writer;

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

            return Math.Max(cellsRows, blocksRows)
                + _titleText == null ? 0 : 1;
        }
    }

    public int Columns
    {
        get 
        {
            int colsByCells = _cells.Count switch
            {
                0 => 1,
                _ => _cells.Select(p => p.Columns).Sum()
            };

            int colByBlocks = _blocks.Count switch
            {
                0 => 1,
                _ => _blocks.Select(p => p.Columns).Sum()
            };

            return Math.Max(colsByCells, colByBlocks);
        }
    }

    public IHeaderCell Title { get; private set; }

    public ICollection<IHeaderCell> Cells => _cells.AsReadOnly();

    public ICollection<IHeaderBlock> Blocks => _blocks.AsReadOnly();

    public void Block(string text, Action<IHeaderBlock> action)
    {
        int colIndex = ColumnIndex + Columns - 1;
        var block = new HeaderBlock(_sheet,
            _cellStyle,
            RowIndex,
            colIndex,
            text);
        action(block);
        block.SetTitle();
        _blocks.Add(block);
    }

    public void Block(Action<IHeaderBlock> action)
        => Block(null, action);

    public void Cell(string text, int rightMerge = 1, int downMerge = 1)
    {
        int rowIndex = _titleText switch
        {
            null => RowIndex,
            _ => RowIndex + 1
        };

        IRow row = _sheet.GetRow(rowIndex) ?? _sheet.CreateRow(rowIndex);
        int colIndex = ColumnIndex + Columns - 1;
        var headerCell = new HeaderCell(row, colIndex);
        headerCell.Text(text, rightMerge, downMerge, _cellStyle);
        _cells.Add(headerCell);
    }

    public void SetTitle()
    {
        if (_titleText == null)
            return;

        var row = _sheet.GetRow(RowIndex) ?? _sheet.CreateRow(RowIndex);
        var headerCell = new HeaderCell(row, ColumnIndex);
        headerCell.Text(_titleText, rightMerge: Columns, cellStyle: _cellStyle);
        Title = headerCell;
    }
}
