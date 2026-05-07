using System;

namespace MetaContext.Extension.NPOI.Reader;

public interface ISheetReader
{
    ISheetReader VerifyHeader(string[] headers, 
        int rowIndex = 0,
        int startColIndex = 0);

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
