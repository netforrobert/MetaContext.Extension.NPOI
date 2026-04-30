using System;
using System.Collections.Generic;
using System.Text;

namespace MetaContext.Extension.NPOI.Writer;

internal interface IPropertyGetterProvider
{
    Func<TObject, TProperty> GetOrAdd<TObject, TProperty>(string key, 
        Func<string, Func<TObject, TProperty>> func);
}
