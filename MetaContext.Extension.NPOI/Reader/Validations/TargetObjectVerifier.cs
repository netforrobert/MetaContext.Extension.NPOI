using System;
using System.Collections.Generic;

using Org.BouncyCastle.Crypto;

namespace MetaContext.Extension.NPOI.Reader.Validations;

internal class TargetObjectVerifier<TTargetObject> : ITargetObjectVerifier<TTargetObject>, IObjectVerifier
{
    private readonly List<Tuple<Func<TTargetObject, ValidationResult>, Func<TTargetObject, object[], string>, bool>> _verifiers = new();

    public ITargetObjectVerifier<TTargetObject> VerifyObject(Func<TTargetObject, ValidationResult> verifyFunc,
        Func<TTargetObject, object[], string> stringFactory,
        bool IsAbortReading)
    {
        _verifiers.Add(new(verifyFunc, stringFactory, IsAbortReading));
        return this;
    }

    public (bool isInvalid, bool isAbortReading) TryVerify(object targetObj, out string message)
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
                return (true, verifier.Item3);
            }
        }

        message = null;
        return (false, false);
    }
}
