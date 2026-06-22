using System;
using System.Collections.Generic;

namespace MetaContext.Extension.NPOI.Reader;

internal class TargetObjectVerifier<TTargetObject> : ITargetObjectVerifier<TTargetObject>, IObjectVerifier
{
    private readonly List<Tuple<Func<TTargetObject, ValidationResult>, Func<TTargetObject, object[], string>>> _verifiers = new();

    public ITargetObjectVerifier<TTargetObject> VerifyObject(Func<TTargetObject, ValidationResult> verifyFunc,
        Func<TTargetObject, object[], string> stringFactory)
    {
        _verifiers.Add(new(verifyFunc, stringFactory));
        return this;
    }

    public bool TryVerify(object targetObj, out string message)
    {
        TTargetObject targetObject = (TTargetObject)targetObj;
        var enumerator = _verifiers.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var verifier = enumerator.Current;
            var result = verifier.Item1.Invoke(targetObject);
            if (result.IsSuccess)
            {
                message = verifier.Item2.Invoke(targetObject, result.Arguments);
                return true;
            }
        }

        message = null;
        return false;
    }
}
