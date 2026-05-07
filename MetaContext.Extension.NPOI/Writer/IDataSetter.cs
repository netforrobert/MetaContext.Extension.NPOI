using System;
using System.Linq.Expressions;

using NPOI.SS.UserModel;

namespace MetaContext.Extension.NPOI.Writer;

public interface IDataSetter<TSourceObject>
{
    IDataSetter<TSourceObject> Set<TProperty, TTargetValue>(string headerName,
        Expression<Func<TSourceObject, TProperty>> expression,
        Func<TProperty, TTargetValue> convertor = null,
        int index = 0);

    IDataSetter<TSourceObject> Set<TProperty>(string headerName,
        Expression<Func<TSourceObject, TProperty>> expression,
        int index = 0);

    IDataSetter<TSourceObject> ExtraSet(Action<IRowSetter, TSourceObject> extraAction);
}
