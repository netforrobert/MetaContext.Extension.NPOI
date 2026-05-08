using System;
using System.Collections.Generic;
using System.Linq;

using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;

namespace MetaContext.Extension.NPOI.Writer;

internal class SheetHeader : ISheetHeader
{
    private readonly List<CellHeader> _cellHeaders = new();
    private readonly ISheet _sheet;
    private readonly ICellStyle _defaultHeaderStyle;
    private int _colIndex;

    public SheetHeader(ISheet sheet,
        int rowIndex, 
        int rows, 
        int colIndex)
    {
        _sheet = sheet;
        RowIndex = rowIndex;
        Rows = rows;
        _colIndex = colIndex;
        StartColIndex = colIndex;
        
        //默认表头样式
        var workbook = _sheet.Workbook;
        ICellStyle headerStyle = workbook.CreateCellStyle();
        IFont font = workbook.CreateFont();
        font.IsBold = true;
        headerStyle.SetFont(font);
        headerStyle.Alignment = HorizontalAlignment.Center;
        headerStyle.SetNormalBorder();
        _defaultHeaderStyle = headerStyle;

        _cellHeaders.Add(new CellHeader(_colIndex, RowIndex, Rows, _sheet, _defaultHeaderStyle));
    }

    public int RowIndex { get; private set; }

    public int Rows { get; private set; }

    public int Cols
    {
        get
        {
            var sumCols = _cellHeaders.Select(p => p.Cols).Sum();
            return sumCols + _colIndex + 1;
        }
    }

    public IEnumerable<string> HeaderTexts
        => _cellHeaders.Select(p => p.HeaderText);

    public int StartColIndex { get; private set; }

    public ISheetHeader Next(int skipCols)
    {
        _colIndex += skipCols;
        var cellHeader = new CellHeader(_colIndex, RowIndex, Rows, _sheet, _defaultHeaderStyle);
        _cellHeaders.Add(cellHeader);
        return this;
    }

    public ISheetHeader Draw(Action<ICellHeader> action)
    {
        var current = _cellHeaders[_cellHeaders.Count - 1];
        action(current);
        return this;
    }
}
