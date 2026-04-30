using System;
using System.Linq.Expressions;

using NPOI.SS.UserModel;

namespace MetaContext.Extension.NPOI.Writer;

public interface IDataWriter<TSourceObject>
{
    IDataWriter<TSourceObject> Write<TProperty, TTargetValue>(Expression<Func<TSourceObject, TProperty>> expression,
        string headerName,
        Func<TProperty, TTargetValue> convertor = null,
        int index = -1);
}
