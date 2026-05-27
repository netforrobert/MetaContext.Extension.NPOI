using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NPOI.SS.UserModel;

namespace MetaContext.Extension.NPOI.Writer;

public interface IRowsWriter<TData>
{
    int Rows { get; }

    int StartRowIndex { get; }

    IRowsWriter<TData> SetItems<TItem>(Func<TData, IEnumerable<TItem>> itemsSelector,
        Func<TData, int> rowsSelector,
        Action<IRowsGroupWriter<TData>> grpWriterAction,
        Action<IRowsWriter<TItem>> itemsAction,
        Func<TItem, int> itemRowsSelector);

    IRowsWriter<TData> Set<TProperty, TTargetValue>(string headerName,
        Func<TData, TProperty> propertyFactory,
        Func<TProperty, TTargetValue> convertor = null,
        int headerIndex = 0);

    IRowsWriter<TData> Set<TProperty>(string headerName,
        Func<TData, TProperty> propertyFactory,
        int headerIndex = 0);

    IRowsWriter<TData> ExtraSet(Action<IRowSetter, TData> extraAction);
}
