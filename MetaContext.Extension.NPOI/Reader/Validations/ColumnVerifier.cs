using System;
using System.Collections.Generic;

using MetaContext.Extension.NPOI.ColumnIndex;

using NPOI.SS.UserModel;

namespace MetaContext.Extension.NPOI.Reader.Validations;

internal class ColumnVerifier : IColumnVerifier
{
    private readonly Dictionary<ColKey, List<ColumnVerifierItem>> _columnVerifiers = new();
    private readonly HashSet<ColKey> _notRequireColKeys = new();
    private readonly IReaderErrorMessageProvider _messageProvider;
    private readonly ColumnIndices _columnIndices;

    public ColumnVerifier(ColumnIndices columnIndices, 
        IReaderErrorMessageProvider messageProvider)
    {
        _columnIndices = columnIndices;
        _messageProvider = messageProvider;
    }

    public IColumnVerifier NotRequire(string columnm, int index = 0)
    {
        _notRequireColKeys.Add(new(columnm, index));
        return this;
    }

    public IColumnVerifier Verify(string column, 
        Func<string, bool> verifyFunc, 
        Func<string, string> errTextFunc, 
        int index = 0)
    {
        ColKey colKey = new(column, index);
        if (!_columnVerifiers.ContainsKey(colKey))
            _columnVerifiers[colKey] = new List<ColumnVerifierItem>();

        _columnVerifiers[colKey].Add(new(verifyFunc, errTextFunc));
        return this;
    }

    public string[] RunVerify(IRow row)
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

        return errorMessages.ToArray();
    }
}
