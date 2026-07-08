using System;

namespace MetaContext.Extension.NPOI.Reader;

public interface IColumnVerifier
{
    IColumnVerifier NotRequire(string columnm, int index = 0);

    IColumnVerifier Verify(string column,
        Func<string, bool> verifyFunc,
        Func<string, string> errTextFunc,
        int index = 0);
}
