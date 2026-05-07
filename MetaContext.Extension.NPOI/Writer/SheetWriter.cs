using System;
using System.Collections.Generic;
using System.Linq;

using MetaContext.Extension.NPOI.ColumIndex;

using NPOI.SS.UserModel;

using Org.BouncyCastle.Asn1.X509;

namespace MetaContext.Extension.NPOI.Writer;

internal class SheetWriter : ISheetWriter
{
    private readonly Dictionary<int, ColumnIndices> _headers = new();
    private readonly ISheet _sheet;

    public SheetWriter(ISheet sheet)
        => _sheet = sheet;

    public ISheetWriter CreateHeader(string[] headers,
        int colStartIndex = 0,
        int rowIndex = 0)
    {
        ColumnIndices indices = new(headers, colStartIndex);
        _headers[rowIndex] = indices;
        var row = _sheet.CreateRow(rowIndex);
        IRowSetter rowWriter = new RowSetter(row, indices);
        foreach (var colIndex in indices.Indices)
            rowWriter.Set(colIndex.Name, colIndex.Name);

        ///表头样式
        var workbook = _sheet.Workbook;
        ICellStyle headerStyle = workbook.CreateCellStyle();
        IFont font = workbook.CreateFont();
        font.IsBold = true;
        headerStyle.SetFont(font);
        headerStyle.Alignment = HorizontalAlignment.Center;
        headerStyle.SetNormalBorder();
        foreach (var colIndex in indices.Indices)
        {
            var cell = row.GetCell(colIndex.StartIndex) ?? row.CreateCell(colIndex.EndIndex);
            cell.CellStyle = headerStyle;
        }

        return this;
    }

    public ISheetWriter UseDefaultAutoWidthSize(int columnsCount)
    {
        if (columnsCount == 0)
        {
            var lastHeader = _headers.OrderByDescending(p => p.Key)
                .Select(p => p.Value).FirstOrDefault();
            columnsCount = lastHeader?.ColumnsCount ?? 0;
        }

        _sheet.UseDefaultAutoWidthSize(columnsCount);
        return this;
    }

    public ISheetWriter Write<TSourceObject>(IEnumerable<TSourceObject> sourceObjects, 
        Action<IDataSetter<TSourceObject>> writerAction, 
        int startRowIndex)
    {
        var lastHeader = _headers.OrderByDescending(p => p.Key)
            .Select(p => p.Value)
            .FirstOrDefault() ?? throw new NotSupportedException("无法获取表头");

        PropertyGetterProvider getterProvider = new();
        int rowIndex = startRowIndex;
        if (rowIndex == -1)
            rowIndex = _headers.Count;

        foreach (var sourceObject in sourceObjects)
        {
            var dataRow = _sheet.GetRow(rowIndex) ?? _sheet.CreateRow(rowIndex);
            IDataSetter<TSourceObject> dataWriter = new DataSetter<TSourceObject>(lastHeader,
                dataRow,
                getterProvider,
                sourceObject);
            writerAction(dataWriter);
            rowIndex++;
        }

        return this;
    }
}
