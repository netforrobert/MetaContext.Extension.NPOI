using System;

namespace MetaContext.Extension.NPOI.Reader.Validations;

internal class ColumnVerifierItem
{
    public ColumnVerifierItem(Func<string, bool> verifyFunc,
        Func<string, string> errTextFunc)
    {
        VerifyFunc = verifyFunc;
        ErrTextFunc = errTextFunc;
    }

    public Func<string, bool> VerifyFunc { get; private set; }

    public Func<string, string> ErrTextFunc { get; private set; }
}