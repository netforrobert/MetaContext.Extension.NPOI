using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetaContext.Extension.NPOI.Reader;

internal class RowValidation : IRowValidation
{
    private readonly List<Func<IRowReader, object>> _valueFactories = new();
    private Func<object[], bool> _verifyFunc;
    private Func<object[], string> _errTextFactory;

    public IRowValidation PickValue<TValue>(string column, 
        int index = -1, 
        Func<string, TValue> valueFactory = null)
    {
        valueFactory ??= x => x.ParseToValue<TValue>();
        _valueFactories.Add(x =>
        {
            string text = x.Read(column, index);
            return valueFactory(text);
        });
        return this;
    }

    public void UseValuesVerify(Func<object[], bool> verifyFunc, 
        Func<object[], string> errTextFactory)
    {
        _verifyFunc = verifyFunc;
        _errTextFactory = errTextFactory;
    }

    public bool IsErrorRow(IRowReader rowReader, out string errMsg)
    {
        var values = _valueFactories.Select(p => p(rowReader)).ToArray();
        if (_verifyFunc(values))
        {
            errMsg = _errTextFactory(values);
            return true;
        }

        errMsg = null;
        return false;
    }

    public void VerifySelf()
    {
        if (_verifyFunc == null)
            throw new ArgumentNullException("未设置行验证方法");

        if (_errTextFactory == null)
            throw new ArgumentNullException("未设置行错误消息方法");
    }
}
