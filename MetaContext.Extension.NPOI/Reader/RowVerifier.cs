using System;
using System.Collections.Generic;
using System.Linq;

using MetaContext.Extension.NPOI.ColumnIndex;

using NPOI.SS.Formula.PTG;
using NPOI.SS.UserModel;

namespace MetaContext.Extension.NPOI.Reader;

public class RowVerifier : IRowVerifier
{
    private readonly Dictionary<ColKey, List<ColumnVerifier>> _columnVerifiers = new();
    private readonly HashSet<ColKey> _notRequireColKeys = new();
    private readonly List<RowValidation> _rowValidations = new();
    private readonly ColumnIndices _columnIndices;
    private readonly IReaderErrorMessageProvider _messageProvider;

    public RowVerifier(ColumnIndices columnIndices, 
        IReaderErrorMessageProvider messageProvider)
    {
        _columnIndices = columnIndices;
        _messageProvider = messageProvider;
    }

    public IRowVerifier NotRequireColumn(string columnm,
        int index)
    {
        _notRequireColKeys.Add(new(columnm, index));
        return this;
    }

    public IRowVerifier VerifyColumn(string column, 
        Func<string, bool> verifyFunc, 
        Func<string, string> errTextFunc, 
        int index = 0)
    {
        ColKey colKey = new(column, index);
        if (!_columnVerifiers.ContainsKey(colKey))
            _columnVerifiers[colKey] = new List<ColumnVerifier>();

        _columnVerifiers[colKey].Add(new(verifyFunc, errTextFunc));
        return this;
    }

    public ErrowRowInfo RunVerify(IRow row)
    {
        var rowReader = new RowReader(row, _columnIndices);
        List<string> errorMessages = new();
        foreach (var item in _columnVerifiers)
        {
            var colKey = item.Key;
            string valueText = rowReader.Read(colKey.Column, colKey.Index);
            if (string.IsNullOrEmpty(valueText))
            {
                if (!_notRequireColKeys.Contains(colKey))
                {
                    string msgTemplate = _messageProvider.GetMessageValue<string>(nameof(IReaderErrorMessageConfig.ColumnNotEmpty));
                    errorMessages.Add(string.Format(msgTemplate, colKey.Column));
                }

                continue;
            }

            foreach (var verifier in item.Value)
            {
                if (verifier.VerifyFunc(valueText))
                {
                    string errmsg = verifier.ErrTextFunc(valueText);
                    errorMessages.Add(errmsg);
                    break;
                }
            }
        }

        if (errorMessages.Count > 0) 
            return new ErrowRowInfo(row.RowNum + 1, errorMessages);

        //行验证
        bool isBreakLoop = false;
        foreach (var rowValidation in _rowValidations)
        {
            (bool isInvalid, isBreakLoop) = rowValidation.IsErrorRow(rowReader, out string errMessage);
            if (isInvalid)
            {
                errorMessages.Add(errMessage);
                break;
            }
        }

        return new ErrowRowInfo(row.RowNum + 1, errorMessages, isBreakLoop);
    }

    public IRowVerifier VerifyRow(Action<IRowValidation> action)
    {
        RowValidation validation = new();
        action(validation);
        _rowValidations.Add(validation);
        return this;
    }

    private class ColumnVerifier
    {
        public ColumnVerifier(Func<string, bool> verifyFunc,
            Func<string, string> errTextFunc)
        {
            VerifyFunc = verifyFunc;
            ErrTextFunc = errTextFunc;
        }

        public Func<string, bool> VerifyFunc { get; private set; }

        public Func<string, string> ErrTextFunc { get; private set; }
    }
}
