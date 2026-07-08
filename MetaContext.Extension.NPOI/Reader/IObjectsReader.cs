using System;

using MetaContext.Extension.NPOI.Header;

using NPOI.SS.UserModel;

namespace MetaContext.Extension.NPOI.Reader;

public interface IObjectsReader<TTargetObject>
{
    void UseRowValidations(Action<IRowVerifier> action);

    void UseObjectValidations(Action<ITargetObjectVerifier<TTargetObject>> action);

    ReadResult <TTargetObject> Read();
}
