using System;

namespace MetaContext.Extension.NPOI.Reader;

public interface IRowValidation
{
    IRowValidation PickValue<TValue>(string column, 
        int index = 0,
        Func<string, TValue> valueFactory = null);

    void UseValuesVerify(Func<object[], bool> verifyFunc, 
        Func<object[], string> errTextFactory);
}
