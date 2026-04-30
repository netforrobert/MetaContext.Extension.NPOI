using System;
using System.Linq.Expressions;

namespace MetaContext.Extension.NPOI.Reader;

public interface IRowReader<TTargetObject>
    where TTargetObject : class, new()
{
    IRowReader<TTargetObject> ForProperty<TProperty>(Expression<Func<TTargetObject, TProperty>> expression,
        string column,
        int index = -1,
        Func<string, TProperty> convertor = null,
        Action<TTargetObject> extraAction = null);

    TTargetObject Read(IRowReader rowReader);
}
