using System;
using System.Collections.Generic;
using System.Text;

namespace MetaContext.Extension.NPOI.Writer;

internal class PropertyGetterProvider : IPropertyGetterProvider
{
    private readonly Dictionary<string, object> _funcs = new();

    public Func<TObject, TProperty> GetOrAdd<TObject, TProperty>(string key, Func<string, Func<TObject, TProperty>> func)
    {
        if (!_funcs.ContainsKey(key))
            _funcs[key] = func(key);

        return (Func<TObject, TProperty>)_funcs[key];
    }
}
