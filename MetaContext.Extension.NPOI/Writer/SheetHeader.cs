using System;
using System.Collections.Generic;
using System.Linq;

using MetaContext.Extension.NPOI.ColumnIndex;

using NPOI.SS.UserModel;

namespace MetaContext.Extension.NPOI.Writer;

internal class SheetHeader : ISheetHeader
{
    private readonly List<IHeaderBlock> _blocks = new();
    private readonly ISheet _sheet;
    private readonly ICellStyle _headerStyle;

    public SheetHeader(ISheet sheet,
        int rowIndex, 
        int colIndex,
        ICellStyle headerStyle)
    {
        _sheet = sheet;
        RowIndex = rowIndex;
        StartColIndex = colIndex;

        //默认表头样式
        if (headerStyle == null)
        {
            var workbook = _sheet.Workbook;
            headerStyle = workbook.CreateCellStyle();
            IFont font = workbook.CreateFont();
            font.IsBold = true;
            headerStyle.SetFont(font);
            headerStyle.Alignment = HorizontalAlignment.Center;
            headerStyle.VerticalAlignment = VerticalAlignment.Center;
            headerStyle.SetNormalBorder();
        }
        _headerStyle = headerStyle;
    }

    public int RowIndex { get; private set; }

    public int Rows
    {
        get
        {
            if (_blocks.Count == 0)
                return 0;

            return _blocks.Select(p => p.Rows).Max();
        }
    }

    public int StartColIndex { get; private set; }

    public int Columns
        => _blocks.Select(p => p.Columns).Sum();

    public IEnumerable<string> HeaderTexts
        => _blocks.SelectMany(p => p.Cells).Select(p => p.HeaderText);

    public ISheetHeader Block(string text, Action<IHeaderBlock> action)
    {
        int colIndex = StartColIndex + Columns;
        var block = new HeaderBlock(_sheet, 
            _headerStyle,
            RowIndex,
            colIndex,
            text);
        action(block);
        block.SetTitle();
        _blocks.Add(block);
        return this;
    }

    public ISheetHeader Block(Action<IHeaderBlock> action)
        => Block(null, action);

    public ISheetHeader Cell(string text, int rightMerge = 1, int downMerge = 1)
    {
        int colIndex = StartColIndex + Columns;
        var block = new HeaderBlock(_sheet,
           _headerStyle,
           RowIndex,
           colIndex,
           null);
        block.Cell(text, rightMerge, downMerge);
        _blocks.Add(block);
        return this;
    }
}
