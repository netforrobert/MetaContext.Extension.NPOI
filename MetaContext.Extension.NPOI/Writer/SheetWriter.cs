using System;
using System.Collections.Generic;
using System.Linq;

using MetaContext.Extension.NPOI.ColumnIndex;
using MetaContext.Extension.NPOI.Header;
using MetaContext.Extension.NPOI.Reader;

using NPOI.POIFS.Crypt.Dsig;
using NPOI.SS.UserModel;

using Org.BouncyCastle.Asn1.X509;

namespace MetaContext.Extension.NPOI.Writer;

internal class SheetWriter : ISheetWriter
{
    private readonly List<ISheetHeader> _sheetHeaders = new();
    private readonly ISheet _sheet;
    private int _rowIndex = -1;

    public SheetWriter(ISheet sheet)
        => _sheet = sheet;

    public ISheetWriter CreateHeader(Action<ISheetHeader> action,
        int colStartIndex,
        int rowIndex,
        ICellStyle cellStyle)
    {
        var header = new SheetHeader(_sheet, 
            rowIndex, 
            colStartIndex,
            cellStyle);
        action(header);
        _sheetHeaders.Add(header);
        return this;
    }

    public ISheetWriter UseDefaultAutoWidthSize(int columnsCount)
    {
        if (columnsCount == 0 && _sheetHeaders.Count > 0)
            columnsCount = _sheetHeaders.Select(p => p.Columns).Max();

        _sheet.UseDefaultAutoWidthSize(columnsCount);
        return this;
    }

    public ISheetWriter Write<TSourceObject>(IEnumerable<TSourceObject> sourceObjects, 
        Action<IRowsWriter<TSourceObject>> writerAction,
        int startRowIndex,
        Func<TSourceObject, int> rowsSelector)
    {
        var lastHeader = _sheetHeaders.OrderByDescending(p => p.RowIndex)
            .FirstOrDefault() ?? throw new NotSupportedException("无法获取表头");
        var headerColumns = lastHeader.HeaderTexts.ToArray();
        var startColIndex = lastHeader.StartColIndex;
        PropertyGetterProvider getterProvider = new();

        _rowIndex = startRowIndex switch
        {
            -1 when _rowIndex == -1 => _sheetHeaders.Select(p => p.Rows).Sum(),
            not -1 => startRowIndex,
            _ => _rowIndex
        };

        ColumnIndices columnIndices = new(headerColumns, startColIndex);
        foreach (var sourceObject in sourceObjects)
        {
            int rows = rowsSelector?.Invoke(sourceObject) ?? 1;
            var dataRow = _sheet.GetRow(_rowIndex) ?? _sheet.CreateRow(_rowIndex);
            var rowWriter = new RowSetter(dataRow, columnIndices, rows);
            IRowsWriter<TSourceObject> rowsWriter = new RowsWriter<TSourceObject>(_rowIndex,
                sourceObject,
                rowWriter,
                _sheet);

            writerAction(rowsWriter);
            _rowIndex += rows;
        }

        return this;
    }
}
