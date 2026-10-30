using System;
using System.Collections.Generic;

namespace MetaContext.Extension.NPOI.Reader.Validations;

internal class ColumnValidation : IColumnValidation
{
    private readonly List<ColVerifier> _colVerifiers = new();
    private readonly IReaderErrorMessageProvider _readerErrorMessageProvider;

    public ColumnValidation(IReaderErrorMessageProvider readerErrorMessageProvider)
    {
        _readerErrorMessageProvider = readerErrorMessageProvider;
    }

    public void ColVerify(string column, Action<IColVerifier> action, int index = 0)
    {
        var colKey = new ColKey(column, index);
        var verifier = new ColVerifier(_readerErrorMessageProvider, colKey);
        action(verifier);
        _colVerifiers.Add(verifier);
    }

    public IEnumerable<string> GetInvalidMessage(IRowReader rowReader)
    {
        foreach (var verifier in _colVerifiers)
        {
            string text = rowReader.Read<string>(verifier.ColKey);
            if (verifier.IsInvalid(text, out string errMsg))
                yield return errMsg;
        }
    }
}
