using System;

using MetaContext.Extension.NPOI.Header;
using MetaContext.Extension.NPOI.Reader.Validations;

using NPOI.SS.UserModel;

namespace MetaContext.Extension.NPOI.Reader;

public interface IObjectsReader<TTargetObject>
{
    void UseRowValidations(Action<IRowVerifier> action);

    void UseObjectValidations(Action<ITargetObjectVerifier<TTargetObject>> action);

    ReadResult<TTargetObject> Read(Action<TTargetObject> extraAction = null,
        int startRowIndex = -1,
        int startColIndex = 0);
}
