using System;

using NPOI.SS.UserModel;

namespace MetaContext.Extension.NPOI.Reader;

public interface IRowVerifier
{
    IRowVerifier NotRequireColumn(string columnm, int index = 0);

    IRowVerifier VerifyColumn(string column,
        Func<string, bool> verifyFunc, 
        Func<string, string> errTextFunc,
        int index = 0);

    IRowVerifier VerifyRow(Action<IColumnsVerifier> action);
}
