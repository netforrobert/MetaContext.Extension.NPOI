using System;
using System.Collections.Generic;
using System.Linq;

using MetaContext.Extension.NPOI.ColumnIndex;
using MetaContext.Extension.NPOI.Header;
using MetaContext.Extension.NPOI.Reader.Validations;

using NPOI.SS.UserModel;

namespace MetaContext.Extension.NPOI.Reader;

public abstract class ObjectsReader<TTargetObject> : IObjectsReader<TTargetObject>
    where TTargetObject : class, new()
{
    private readonly ReaderErrorMessageProvider _errorMessageProvider = new();
    private readonly TargetObjectVerifier<TTargetObject> _objectVerifier = new();
    private readonly List<Action<IRowVerifier>> _appendedRowValidations = new();
    private readonly List<Action<ITargetObjectVerifier<TTargetObject>>> _appendedObjValidations = new();

    private readonly RowVerifier _rowVerifier;
    private readonly ISheet _sheet;
    private readonly IEnumerable<HeaderInfo> _headers;
    private readonly ColumnIndices _columnIndices;


    protected ObjectsReader(ISheet sheet,
        IEnumerable<HeaderInfo> headers)
    {
        _sheet = sheet;
        _headers = headers;

        int rowIndex = _headers.Select(p => p.RowIndex).Max();
        var cols = _headers.Where(p => p.RowIndex == rowIndex)
            .Select(p => p.HeaderText).ToArray();
        _columnIndices = new(cols);
        _rowVerifier = new(_columnIndices, _errorMessageProvider);
        ConfigErrorMessage(_errorMessageProvider);
    }

    public ReadResult<TTargetObject> Read(Action<TTargetObject> extraAction = null,
        int startRowIndex = -1,
        int startColIndex = 0)
    {
        _rowVerifier.VerifyColumn(ColumnVerify);
        _rowVerifier.VerifyRow(ColumnsVerify);
        foreach (var action in _appendedRowValidations)
            action(_rowVerifier);

        TTargetObjectVerify(_objectVerifier);
        foreach (var action in _appendedObjValidations)
            action(_objectVerifier);

        //处理表头校验
        var headerRowIndices = _headers.Select(p => p.RowIndex).Distinct().OrderBy(p => p).ToArray();
        foreach (var headerRowIndex in headerRowIndices)
        {
            var headerRow = _sheet.GetRow(headerRowIndex);
            if (headerRow == null)
            {
                string message = _errorMessageProvider.GetMessageValue<string>(nameof(IReaderErrorMessageConfig.NullHeader));
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
                Func<IEnumerable<ErrorHeaderItem>, List<string>> messageFactory = _errorMessageProvider
                    .GetMessageValue<Func<IEnumerable<ErrorHeaderItem>, List<string>>>(nameof(IReaderErrorMessageConfig.ErrorHeaders));
                ErrowRowInfo errowRowInfo = new(headerRowIndex, messageFactory(errorHeaderItems));
                return new ReadResult<TTargetObject>(errowRowInfo);
            }
        }

        List<ErrowRowInfo> errowRowInfos = new();
        List<TTargetObject> targetObjects = new();
        int processedCount = 0;
        int successedCount = 0;
        if (startRowIndex == -1)
            startRowIndex = _headers.Select(p => p.RowIndex).Max() + 1;

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
            int errsCount = errInfo.ErrMessages.Count;
            if (errInfo.ErrMessages.Count > 0)
            {
                errowRowInfos.Add(errInfo);
                if (errInfo.IsAbortReading)
                    break;

                continue;
            }

            var objReader = new RowReader<TTargetObject>();
            var rowReader = new RowReader(row, _columnIndices);
            ReadTargetObject(objReader);
            var targetObject = objReader.Read(rowReader);
            extraAction?.Invoke(targetObject);

            //校验对象
            (bool isInvalid, bool isAbortReading) = _objectVerifier.TryVerify(targetObject, out string message);
            if (isInvalid)
            {
                errowRowInfos.Add(new(rowIndex + 1, message));
                if (isAbortReading)
                    break;

                continue;
            }

            targetObjects.Add(targetObject);
            successedCount++;
        }

        return new ReadResult<TTargetObject>(targetObjects,
            processedCount,
            successedCount,
            errowRowInfos);

        throw new NotImplementedException();
    }

    public void UseObjectValidations(Action<ITargetObjectVerifier<TTargetObject>> action)
        => _appendedObjValidations.Add(action);

    public void UseRowValidations(Action<IRowVerifier> action)
        => _appendedRowValidations.Add(action);

    protected abstract void ColumnVerify(IColumnVerifier columnVerifier);

    protected abstract void ColumnsVerify(IColumnsVerifier columnsVerifier);

    protected abstract void ReadTargetObject(IRowReader<TTargetObject> rowReader);

    protected virtual void TTargetObjectVerify(ITargetObjectVerifier<TTargetObject> objectVerifier)
    { 
    }

    protected virtual void ConfigErrorMessage(IReaderErrorMessageConfig errorMessageConfig)
    { 
    }
}
