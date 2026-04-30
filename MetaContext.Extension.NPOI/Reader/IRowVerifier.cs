using System;

using NPOI.SS.UserModel;

namespace MetaContext.Extension.NPOI.Reader;

public interface IRowVerifier
{
    IRowVerifier NotRequireColumn(string columnm, int index = -1);

    IRowVerifier VerifyColumn(string column,
        Func<string, bool> verifyFunc, 
        Func<string, string> errTextFunc,
        int index = -1);

    IRowVerifier VerifyRow(Func<IRowReader, bool> verifyFunc, Func<IRowReader, string> errTextFunc);

    ErrowRowInfo RunVerify(IRow row);
}
