using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NPOI.SS.UserModel;

namespace MetaContext.Extension.NPOI.Writer;

public interface IRowsWriter<TData>
{
    int Rows { get; }

    int StartRowIndex { get; }

    IRowsWriter<TData> SetItems<TItem>(string headerName,
        Expression<Func<TData, IEnumerable<TItem>>> dataSourceExp,
        Expression<Func<TData, int>> rowsSelector,
        Action<IRowsGroupWriter<TData>> grpWriterAction,
        Action<IRowsWriter<TItem>> itemsAction);

    IRowsWriter<TData> Set<TProperty, TTargetValue>(string headerName,
        Expression<Func<TData, TProperty>> expression,
        Func<TProperty, TTargetValue> convertor = null,
        int headerIndex = 0);

    IRowsWriter<TData> Set<TProperty>(string headerName,
        Expression<Func<TData, TProperty>> expression,
        int headerIndex = 0);

    IRowsWriter<TData> ExtraSet(Action<IRowSetter, TData> extraAction);
}
