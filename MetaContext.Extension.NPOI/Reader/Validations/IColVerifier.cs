using System;

namespace MetaContext.Extension.NPOI.Reader.Validations;

public interface IColVerifier
{
    ColKey ColKey { get; }

    bool IsRequired { get; }

    IColVerifier NotRequired();

    IColVerifier Verify(Func<string, bool> verifyFunc, 
        Func<ColValidationArguments,string> errTextFunc);
}
