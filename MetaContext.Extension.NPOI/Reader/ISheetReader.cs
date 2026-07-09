using System;
using System.Collections.Generic;

using MetaContext.Extension.NPOI.Header;
using MetaContext.Extension.NPOI.Reader.Validations;

using NPOI.SS.UserModel;

namespace MetaContext.Extension.NPOI.Reader;

public interface ISheetReader
{
    ISheetReader UseValidation(Action<IRowVerifier> action);

    ReadResult<TTargetObject> Read<TTargetObject>(Action<IRowReader<TTargetObject>> readerAction,
        int startRowIndex = 1,
        int startColIndex = 0,
        Action<ITargetObjectVerifier<TTargetObject>> objectVerify = null)
        where TTargetObject : class, new();

    ReadResult<TTargetObject> Read<TTargetObject>(Func<IRowReader, TTargetObject> targetFactory,
        int startRowIndex = 1,
        int startColIndex = 0,
        Action<ITargetObjectVerifier<TTargetObject>> objectVerify = null);
}
