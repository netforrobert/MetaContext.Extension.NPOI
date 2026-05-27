using System;
using System.Collections.Generic;
using System.Linq;

using MetaContext.Extension.NPOI.ColumnIndex;

using NPOI.POIFS.Crypt.Dsig;
using NPOI.SS.UserModel;

using Org.BouncyCastle.Asn1.X509;

namespace MetaContext.Extension.NPOI.Writer;

internal class SheetWriter : ISheetWriter
{
    private readonly List<ISheetHeader> _sheetHeaders = new();
    private readonly ISheet _sheet;

    public SheetWriter(ISheet sheet)
        => _sheet = sheet;

    public ISheetWriter CreateHeader(Action<ISheetHeader> action,
        int colStartIndex = 0,
        int rowIndex = 0,
        int rows = 1)
    {
        var header = new SheetHeader(_sheet, rowIndex, rows, colStartIndex);
        action(header);
        _sheetHeaders.Add(header);
        return this;
    }

    public ISheetWriter UseDefaultAutoWidthSize(int columnsCount)
    {
        if (columnsCount == 0 && _sheetHeaders.Count > 0)
            columnsCount = _sheetHeaders.Select(p => p.Cols).Max();

        _sheet.UseDefaultAutoWidthSize(columnsCount);
        return this;
    }

    public ISheetWriter Write<TSourceObject>(IEnumerable<TSourceObject> sourceObjects, 
        Action<IDataSetter<TSourceObject>> writerAction, 
        int startRowIndex)
    {
        var lastHeader = _sheetHeaders.OrderByDescending(p => p.RowIndex)
            .FirstOrDefault() ?? throw new NotSupportedException("无法获取表头");

        PropertyGetterProvider getterProvider = new();
        int rowIndex = startRowIndex;
        if (rowIndex == -1)
            rowIndex = _sheetHeaders.Count;

        var columns = lastHeader.HeaderTexts.ToArray();
        ColumnIndices columnIndices = new(columns, lastHeader.StartColIndex);
        foreach (var sourceObject in sourceObjects)
        {
            var dataRow = _sheet.GetRow(rowIndex) ?? _sheet.CreateRow(rowIndex);
            IDataSetter<TSourceObject> dataWriter = new DataSetter<TSourceObject>(columnIndices,
                dataRow,
                getterProvider,
                sourceObject);
            writerAction(dataWriter);
            rowIndex++;
        }

        return this;
    }

    public ISheetWriter Write<TSourceObject>(IEnumerable<TSourceObject> sourceObjects, 
        Action<IRowsWriter<TSourceObject>> writerAction,
        int startRowIndex,
        Func<TSourceObject, int> rowsSelector)
    {
        var lastHeader = _sheetHeaders.OrderByDescending(p => p.RowIndex)
            .FirstOrDefault() ?? throw new NotSupportedException("无法获取表头");

        PropertyGetterProvider getterProvider = new();
        int rowIndex = startRowIndex;
        if (rowIndex == -1)
            rowIndex = _sheetHeaders.Count;

        var columns = lastHeader.HeaderTexts.ToArray();
        ColumnIndices columnIndices = new(columns, lastHeader.StartColIndex);
        foreach (var sourceObject in sourceObjects)
        {
            int rows = rowsSelector?.Invoke(sourceObject) ?? 1;
            var dataRow = _sheet.GetRow(rowIndex) ?? _sheet.CreateRow(rowIndex);
            var rowWriter = new RowSetter(dataRow, columnIndices, rows);
            IRowsWriter<TSourceObject> rowsWriter = new RowsWriter<TSourceObject>(rowIndex,
                sourceObject,
                rowWriter,
                _sheet);

            writerAction(rowsWriter);
            rowIndex += rows;
        }

        return this;
    }
}
