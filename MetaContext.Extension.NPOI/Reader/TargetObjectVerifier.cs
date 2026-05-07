using System;
using System.Collections.Generic;

namespace MetaContext.Extension.NPOI.Reader;

internal class TargetObjectVerifier<TTargetObject> : ITargetObjectVerifier<TTargetObject>, IObjectVerifier
{
    private readonly List<Tuple<Func<TTargetObject, bool>, Func<TTargetObject, string>>> _verifiers = new();

    public ITargetObjectVerifier<TTargetObject> VerifyObject(Func<TTargetObject, bool> verifyFunc, 
        Func<TTargetObject, string> stringFactory)
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
            if (verifier.Item1.Invoke(targetObject))
            {
                message = verifier.Item2.Invoke(targetObject);
                return true;
            }
        }

        message = null;
        return false;
    }
}
