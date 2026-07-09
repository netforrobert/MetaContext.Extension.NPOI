using System;

namespace MetaContext.Extension.NPOI.Reader.Validations;

public interface ITargetObjectVerifier<TTargetObject>
{
    ITargetObjectVerifier<TTargetObject> VerifyObject(Func<TTargetObject, ValidationResult> verifyFunc,
        Func<TTargetObject, object[], string> stringFactory,
        bool IsAbortReading = false);
}
