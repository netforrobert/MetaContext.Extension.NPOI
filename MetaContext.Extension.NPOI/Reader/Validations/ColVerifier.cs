using System;
using System.Collections.Generic;

namespace MetaContext.Extension.NPOI.Reader.Validations;

internal class ColVerifier : IColVerifier
{
    private readonly List<ColVerifyItem> _verifyItems = new();
    private readonly IReaderErrorMessageProvider _errorMessageProvider;

    public ColVerifier(IReaderErrorMessageProvider errorMessageProvider,
        ColKey colKey)
    {
        _errorMessageProvider = errorMessageProvider;
        ColKey = colKey;
        IsRequired = true;
    }

    public ColKey ColKey { get; private set; }

    public bool IsRequired { get; private set; }

    public IColVerifier NotRequired()
    {
        IsRequired = false;
        return this;
    }

    public IColVerifier Verify(Func<string, bool> verifyFunc, Func<ColValidationArguments, string> errTextFunc)
    {
        _verifyItems.Add(new(verifyFunc, errTextFunc));
        return this;
    }

    public bool IsInvalid(string value, out string errMessage)
    {
        bool UseVerify(string text, out string errMsg)
        {
            if (IsRequired)
            {
                string msgTemplate = _errorMessageProvider.GetMessageValue<string>(nameof(IReaderErrorMessageConfig.ColumnNotEmpty));
                _verifyItems.Insert(0, new(string.IsNullOrEmpty, args => string.Format(msgTemplate, ColKey.Column)));
            }

            var enumerator = _verifyItems.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var verifier = enumerator.Current;
                if (verifier.VerifyFunc(text))
                {
                    errMsg = verifier.ErrTextFunc(new(ColKey, text));
                    return true;
                }
            }

            errMsg = null;
            return false;
        }

        switch (IsRequired)
        {
            case true:
                return UseVerify(value, out errMessage);
            case false when !string.IsNullOrEmpty(value):
                return UseVerify(value, out errMessage);
            default: 
                errMessage = null;
                return true;
        }
    }

    private class ColVerifyItem
    {
        public ColVerifyItem(Func<string, bool> verifyFunc, Func<ColValidationArguments, string> errTextFunc)
        {
            VerifyFunc = verifyFunc;
            ErrTextFunc = errTextFunc;
        }

        public Func<string, bool> VerifyFunc { get; private set; }

        public Func<ColValidationArguments, string> ErrTextFunc { get; private set; }
    }
}
