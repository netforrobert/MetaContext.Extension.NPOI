using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetaContext.Extension.NPOI.Reader;

internal class RowValidation : IRowValidation
{
    private readonly List<Tuple<Func<IRowReader, ValidationResult>, Func<IRowReader, object[], string>, bool>> _verifiers = new();

    public void UseValuesVerify(Func<IRowReader, ValidationResult> verfierfunc,
        Func<IRowReader, object[], string> messageFactory,
        bool isBreakLoop)
    {
        _verifiers.Add(new(verfierfunc, messageFactory, isBreakLoop));
    }

    public (bool isInvalid, bool isBreakLoop) IsErrorRow(IRowReader rowReader, out string errMsg)
    {
        foreach (var verifier in _verifiers)
        {
            var result = verifier.Item1(rowReader);
            if (result.IsSuccess)
            {
                errMsg = verifier.Item2(rowReader, result.Arguments);
                return (true, verifier.Item3);
            }
        }

        errMsg = null;
        return (false, false);
    }
}
