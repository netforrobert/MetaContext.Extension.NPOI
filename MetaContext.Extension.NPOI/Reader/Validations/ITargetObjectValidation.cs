using System;

namespace MetaContext.Extension.NPOI.Reader.Validations;

public interface ITargetObjectValidation<TTargetObject>
{
    ITargetObjectValidation<TTargetObject> Verify(Func<TTargetObject, ValidationResult> verifyFunc,
        Func<TTargetObject, object[], string> stringFactory,
        bool IsAbortReading = false);
}
