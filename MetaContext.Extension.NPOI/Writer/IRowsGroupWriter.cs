using System;
using System.Linq.Expressions;

namespace MetaContext.Extension.NPOI.Writer;

public interface IRowsGroupWriter<TData>
{
    IRowsWriter<TData> Set<TProperty, TTargetValue>(string headerName,
        Expression<Func<TData, TProperty>> propSelector,
        Action<ICellWriter, TProperty> writerAction = null,
        int headerIndex = 0);
}
