using System;
using System.Collections.Generic;
using System.Linq;

using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;

using static NPOI.HSSF.Util.HSSFColor;

namespace MetaContext.Extension.NPOI.Writer;

internal class SheetHeader : ISheetHeader
{
    private readonly List<IRegionBlock> _blocks = new();
    private readonly ISheet _sheet;
    private readonly ICellStyle _defaultHeaderStyle;

    public SheetHeader(ISheet sheet,
        int rowIndex, 
        int rows, 
        int colIndex)
    {
        _sheet = sheet;
        RowIndex = rowIndex;
        Rows = rows;
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
    }

    public int RowIndex { get; private set; }

    public int Rows { get; private set; }

    public int StartColIndex { get; private set; }

    public int Cols 
    {
        get
        {
            if (_blocks.Count == 0)
                return 1;

            return _blocks.Select(p => p.Cols).Sum();
        }
    }

    public IEnumerable<string> HeaderTexts => throw new NotImplementedException();

    public ISheetHeader Block(string text, Action<IRegionBlock> action)
    {
        int colIndex = StartColIndex + Cols;
        var block = new RegionBlock(_sheet, 
            _defaultHeaderStyle,
            RowIndex + 1,
            colIndex);
        action(block);

        var row = _sheet.GetRow(RowIndex) ?? _sheet.CreateRow(RowIndex);
        var regionCell = new RegionCell(row, colIndex);
        regionCell.SetValue(text, rightMerge: block.Cols);
        return this;
    }

    public ISheetHeader Col(string text, int rightMerge = 1, int downMerge = 1)
    {
        var row = _sheet.GetRow(RowIndex) ?? _sheet.CreateRow(RowIndex);
        int colIndex = StartColIndex + Cols;
        var regionCell = new RegionCell(row, colIndex);
        regionCell.SetValue(text, rightMerge, downMerge, _defaultHeaderStyle);
        return this;
    }
}
