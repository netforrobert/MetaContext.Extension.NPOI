using System;

namespace MetaContext.Extension.NPOI.Reader;

internal class ColumnVerifier : IColumnVerifier
{
    private readonly IRowVerifier _rowVerifier;

    public ColumnVerifier(IRowVerifier rowVerifier)
    {
        _rowVerifier = rowVerifier;
    }

    public IColumnVerifier NotRequire(string columnm, int index = 0)
    {
        _rowVerifier.NotRequireColumn(columnm, index);
        return this;
    }

    public IColumnVerifier Verify(string column, 
        Func<string, bool> verifyFunc, 
        Func<string, string> errTextFunc, 
        int index = 0)
    {
        _rowVerifier.VerifyColumn(column, verifyFunc, errTextFunc, index);
        return this;
    }
}
