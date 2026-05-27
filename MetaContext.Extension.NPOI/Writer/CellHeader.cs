using System;

using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace MetaContext.Extension.NPOI.Writer;

internal class CellHeader : ICellHeader
{
    private readonly ICellStyle _defaultHeaderStyle;

    private readonly ICellWriter _cellWriter;

    public CellHeader(int startColIndex,
        int startRowIndex,
        int maxRows,
        int maxColumns,
        ISheet sheet,
        ICellStyle defaultHeaderStyle)
    {
        _defaultHeaderStyle = defaultHeaderStyle;
        var headerRow = sheet.GetRow(startRowIndex) ?? sheet.CreateRow(startRowIndex);
        _cellWriter = new CellWriter(headerRow,
            startColIndex,
            maxRows,
            maxColumns);
    }

    public int Cols => _cellWriter.Cols;

    public int Rows => _cellWriter.Rows;

    public string HeaderText { get; private set; }


    public ICellHeader DownMerge(int rows)
    {
        _cellWriter.DownMerge(rows);
        return this;
    }

    public ICellHeader RightMerge(int cols)
    {
        _cellWriter.RightMerge(cols);
        return this;
    }

    public void SetHeaderText(string text, ICellStyle headerStyle)
    {
        headerStyle ??= _defaultHeaderStyle;
        _cellWriter.SetStyle(headerStyle)
            .SetValue(text);
    }
}
