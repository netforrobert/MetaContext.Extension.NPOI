using System;
using System.Collections.Generic;
using System.Linq;

using MetaContext.Extension.NPOI.ColumnIndex;
using MetaContext.Extension.NPOI.Header;

using NPOI.SS.UserModel;

namespace MetaContext.Extension.NPOI.Reader;

internal class SheetReader : ISheetReader
{
    private RowVerifier _rowVerifier;
    private readonly ISheet _sheet;
    private readonly IEnumerable<HeaderInfo> _headers;
    private readonly IReaderErrorMessageProvider _messageProvider;
    private readonly ColumnIndices _columnIndices;

    public SheetReader(ISheet sheet, 
        IEnumerable<HeaderInfo> headers,
        IReaderErrorMessageProvider messageProvider)
    {
        _sheet = sheet;
        _headers = headers;
        _messageProvider = messageProvider;

        int rowIndex = _headers.Select(p => p.RowIndex).Max();
        var cols = _headers.Where(p => p.RowIndex == rowIndex)
            .Select(p => p.HeaderText).ToArray();
        _columnIndices = new(cols);
    }

    public ISheetReader UseValidation(Action<IRowVerifier> action)
    {
        _rowVerifier ??= new RowVerifier(_columnIndices, _messageProvider);
        action(_rowVerifier);
        return this;
    }

    public ReadResult<TTargetObject> Read<TTargetObject>(Action<IRowReader<TTargetObject>> readerAction,
        int startRowIndex = 1, 
        int startColIndex = 0,
        Action<ITargetObjectVerifier<TTargetObject>> objectVerify = null)
        where TTargetObject : class, new()
    {
        IRowReader<TTargetObject> rowTReader = new RowReader<TTargetObject>();
        readerAction(rowTReader);

        TTargetObject ObjectFactory(IRowReader rowReader) => rowTReader.Read(rowReader);
        return Read(ObjectFactory, startRowIndex, startColIndex, objectVerify);
    }

    public ReadResult<TTargetObject> Read<TTargetObject>(Func<IRowReader, TTargetObject> targetFactory,
        int startRowIndex = 1, 
        int startColIndex = 0,
        Action<ITargetObjectVerifier<TTargetObject>> objectVerify = null)
    {
        //处理表头校验
        var headerRowIndices = _headers.Select(p => p.RowIndex).Distinct().OrderBy(p => p).ToArray();
        foreach (var headerRowIndex in headerRowIndices)
        {
            var headerRow = _sheet.GetRow(headerRowIndex);
            if (headerRow == null)
            {
                string message = _messageProvider.GetMessageValue<string>(nameof(IReaderErrorMessageConfig.NullHeader));
                return new ReadResult<TTargetObject>(new(headerRowIndex, message));
            }

            var headers = _headers.Where(p => p.RowIndex == headerRowIndex);
            List<ErrorHeaderItem> errorHeaderItems = new();
            foreach (var header in headers)
            {
                if (string.IsNullOrEmpty(header.HeaderText))
                    continue;

                string text = headerRow.GetCell(header.ColumnIndex)?.ToString();
                
                if (header.HeaderText != text)
                    errorHeaderItems.Add(new(header.ColumnIndex, header.HeaderText, text));
            }

            if (errorHeaderItems.Count > 0)
            {
                Func<IEnumerable<ErrorHeaderItem>, List<string>> messageFactory = _messageProvider
                    .GetMessageValue<Func<IEnumerable<ErrorHeaderItem>, List<string>>>(nameof(IReaderErrorMessageConfig.ErrorHeaders));
                ErrowRowInfo errowRowInfo = new(headerRowIndex, messageFactory(errorHeaderItems));
                return new ReadResult<TTargetObject>(errowRowInfo);
            }
        }

        List<ErrowRowInfo> errowRowInfos = new();
        List<TTargetObject> targetObjects = new();
        int processedCount = 0;
        int successedCount = 0;
        TargetObjectVerifier<TTargetObject> objectVerifier = new();
        objectVerify?.Invoke(objectVerifier);
        for (int rowIndex = startRowIndex; rowIndex <= _sheet.LastRowNum; rowIndex++)
        {
            var row = _sheet.GetRow(rowIndex);
            if (row == null
                || row.GetCell(startColIndex) == null
                || row.GetCell(startColIndex).ToString().Trim() == "")
                break;

            processedCount++;
            //校验行
            var errInfo = _rowVerifier.RunVerify(row);
            if (errInfo.ErrMessages.Count > 0)
            {
                errowRowInfos.Add(errInfo);
                continue;
            }
            TTargetObject targetObject = targetFactory(new RowReader(row, _columnIndices));
            //校验对象
            if (objectVerifier.TryVerify(targetObject, out string message))
            {
                errowRowInfos.Add(new(rowIndex + 1, message));
                continue;
            }

            targetObjects.Add(targetObject);
            successedCount++;
        }

        return new ReadResult<TTargetObject>(targetObjects,
            processedCount,
            successedCount,
            errowRowInfos);
    }
}
