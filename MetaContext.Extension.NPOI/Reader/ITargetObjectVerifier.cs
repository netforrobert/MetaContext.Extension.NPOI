using System;

namespace MetaContext.Extension.NPOI.Reader;

public interface ITargetObjectVerifier<TTargetObject>
{
    ITargetObjectVerifier<TTargetObject> VerifyObject(Func<TTargetObject, ValidationResult> verifyFunc,
        Func<TTargetObject, object[], string> stringFactory);
}
