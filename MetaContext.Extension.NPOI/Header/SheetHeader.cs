using System;
using System.Collections.Generic;
using System.Linq;

using MetaContext.Extension.NPOI.ColumnIndex;
using MetaContext.Extension.NPOI.Writer;

using NPOI.SS.UserModel;

namespace MetaContext.Extension.NPOI.Header;

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
        headerStyle ??= CreateDefaultHeaderStyle();
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

    public IEnumerable<HeaderInfo> Headers
    {
        get
        {
            static IEnumerable<HeaderInfo> EachBlock(IHeaderBlock block)
            {
                if (block.Title != null)
                    yield return new HeaderInfo(block.Title);

                foreach (var cell in block.Cells)
                    yield return new HeaderInfo(cell);

                foreach (var blk in block.Blocks)
                {
                    var headers = EachBlock(blk);
                    foreach (var header in headers)
                        yield return header;
                }
            }

            foreach (var block in _blocks)
            {
                var headers = EachBlock(block);
                foreach (var header in headers)
                    yield return header;
            }
        }
    }

    public ISheetHeader Block(string text, Action<IHeaderBlock> action, ICellStyle headerStyle = null)
    {
        int colIndex = StartColIndex + Columns;
        var block = new HeaderBlock(_sheet, 
            _headerStyle,
            RowIndex,
            colIndex,
            text);
        action(block);
        headerStyle ??= _headerStyle;
        block.SetTitle(headerStyle);
        _blocks.Add(block);
        return this;
    }

    public ISheetHeader Cell(string text, int rightMerge = 1, int downMerge = 1, ICellStyle headerStyle = null)
    {
        int colIndex = StartColIndex + Columns;
        var block = new HeaderBlock(_sheet,
           _headerStyle,
           RowIndex,
           colIndex,
           null);
        headerStyle ??= _headerStyle;
        block.Cell(text, rightMerge, downMerge, headerStyle);
        _blocks.Add(block);
        return this;
    }

    public ICellStyle CreateDefaultHeaderStyle()
    {
        var workbook = _sheet.Workbook;
        var headerStyle = workbook.CreateCellStyle();
        IFont font = workbook.CreateFont();
        font.IsBold = true;
        headerStyle.SetFont(font);
        headerStyle.Alignment = HorizontalAlignment.Center;
        headerStyle.VerticalAlignment = VerticalAlignment.Center;
        headerStyle.SetNormalBorder();
        return headerStyle;
    }
}
