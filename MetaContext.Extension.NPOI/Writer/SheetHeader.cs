using System;
using System.Collections.Generic;
using System.Linq;

using NPOI.SS.UserModel;

namespace MetaContext.Extension.NPOI.Writer;

internal class SheetHeader : ISheetHeader
{
    private readonly List<IHeaderBlock> _blocks = new();
    private readonly ISheet _sheet;
    private readonly ICellStyle _defaultHeaderStyle;

    public SheetHeader(ISheet sheet,
        int rowIndex, 
        int colIndex)
    {
        _sheet = sheet;
        RowIndex = rowIndex;
        StartColIndex = colIndex;
        
        //默认表头样式
        var workbook = _sheet.Workbook;
        ICellStyle headerStyle = workbook.CreateCellStyle();
        IFont font = workbook.CreateFont();
        font.IsBold = true;
        headerStyle.SetFont(font);
        headerStyle.Alignment = HorizontalAlignment.Center;
        headerStyle.VerticalAlignment = VerticalAlignment.Center;
        headerStyle.SetNormalBorder();
        _defaultHeaderStyle = headerStyle;
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

    public int Cols
        => _blocks.Select(p => p.Columns).Sum();

    public IEnumerable<string> HeaderTexts => throw new NotImplementedException();

    public ISheetHeader Block(string text, Action<IHeaderBlock> action)
    {
        int colIndex = StartColIndex + Cols;
        var block = new HeaderBlock(_sheet, 
            _defaultHeaderStyle,
            RowIndex + 1,
            colIndex,
            text);
        action(block);
        block.SetTitle();
        _blocks.Add(block);
        return this;
    }

    public ISheetHeader Block(Action<IHeaderBlock> action)
        => Block(null, action);
}
