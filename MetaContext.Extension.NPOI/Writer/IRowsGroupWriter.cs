using System;
using System.Linq.Expressions;

namespace MetaContext.Extension.NPOI.Writer;

public interface IRowsGroupWriter<TData>
{
    IRowsGroupWriter<TData> Set<TProperty, TTargetValue>(string headerName,
        Func<TData, TProperty> propSelector,
        Action<ICellWriter, TProperty> writerAction = null,
        int headerIndex = 0);
}
