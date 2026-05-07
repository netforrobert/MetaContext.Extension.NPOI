using System;

namespace MetaContext.Extension.NPOI.Reader;

public interface ITargetObjectVerifier<TTargetObject>
{
    ITargetObjectVerifier<TTargetObject> VerifyObject(Func<TTargetObject, bool> verifyFunc,
        Func<TTargetObject, string> stringFactory);
}
