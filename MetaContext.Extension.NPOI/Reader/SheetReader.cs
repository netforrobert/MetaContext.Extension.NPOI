using System;
using System.Collections.Generic;
using System.Linq;

using MetaContext.Extension.NPOI.ColumIndex;

using NPOI.SS.UserModel;

namespace MetaContext.Extension.NPOI.Reader;

internal class SheetReader : ISheetReader
{
    private readonly List<IHeaderVerifier> _headerVerifiers = new();
    private IRowVerifier _rowVerifier;
    private readonly ISheet _sheet;
    private readonly IReaderErrorMessageProvider _messageProvider;

    public SheetReader(ISheet sheet, 
        IReaderErrorMessageProvider messageProvider)
    {
        _sheet = sheet;
        _messageProvider = messageProvider;
    }

    public ISheetReader UseValidation(Action<IRowVerifier> action)
    {
        if (LastHeader == null)
            throw new NotSupportedException("未验证表头");

        _rowVerifier ??= new RowVerifier(LastHeader, _messageProvider);
        action(_rowVerifier);
        return this;
    }

    public ISheetReader VerifyHeader(string[] headers, 
        int rowIndex, 
        int startColIndex)
    {
        var headerVerifier = new HeaderVerifier(rowIndex, new ColumnIndices(headers, startColIndex));
        _headerVerifiers.Add(headerVerifier);
        return this;
    }

    public ReadResult<TTargetObject> Read<TTargetObject>(Action<IRowReader<TTargetObject>> readerAction,
        int startRowIndex = 1, 
        int startColIndex = 0)
        where TTargetObject : class, new()
    {
        IRowReader<TTargetObject> rowTReader = new RowReader<TTargetObject>();
        readerAction(rowTReader);

        TTargetObject ObjectFactory(IRowReader rowReader) => rowTReader.Read(rowReader);
        return Read(ObjectFactory, startRowIndex, startColIndex);
    }

    public ReadResult<TTargetObject> Read<TTargetObject>(Func<IRowReader, TTargetObject> targetFactory,
        int startRowIndex = 1, 
        int startColIndex = 0)
    {
        var enumerator = _headerVerifiers.GetEnumerator();
        //处理表头校验
        while (enumerator.MoveNext())
        {
            var headerVerifer = enumerator.Current;
            var headerRow = _sheet.GetRow(headerVerifer.RowIndex);
            if (headerRow == null)
            {
                string message = _messageProvider.GetMessageValue<string>(nameof(IReaderErrorMessageConfig.NullHeader));
                return new ReadResult<TTargetObject>(new(headerVerifer.RowIndex, message));
            }

            var errorHeaderInfo = headerVerifer.Verify(headerRow);
            if (errorHeaderInfo.IsError)
            {
                Func<IEnumerable<ErrorHeaderItem>, List<string>> messageFactory = _messageProvider
                    .GetMessageValue<Func<IEnumerable<ErrorHeaderItem>, List<string>>>(nameof(IReaderErrorMessageConfig.ErrorHeaders));
                ErrowRowInfo errowRowInfo = new(headerVerifer.RowIndex, messageFactory(errorHeaderInfo.ErrorItems));
                return new ReadResult<TTargetObject>(errowRowInfo);
            }
        }

        if (LastHeader == null)
            throw new NotSupportedException("未验证表头");

        List<ErrowRowInfo> errowRowInfos = new();
        List<TTargetObject> targetObjects = new();
        int processedCount = 0;
        int successedCount = 0;
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
            TTargetObject targetObject = targetFactory(new RowReader(row, LastHeader));
            targetObjects.Add(targetObject);
            successedCount++;
        }

        return new ReadResult<TTargetObject>(targetObjects,
            processedCount,
            successedCount,
            errowRowInfos);
    }

    private ColumnIndices LastHeader
        => _headerVerifiers.OrderByDescending(p => p.RowIndex)
        .Select(p => p.ColIndices)
        .FirstOrDefault();
}
