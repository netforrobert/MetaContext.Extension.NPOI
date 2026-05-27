using System;
using MetaContext.Extension.NPOI.ColumnIndex;

using NPOI.SS.UserModel;

namespace MetaContext.Extension.NPOI.Writer;

internal class RowsGroupWriter<TData> : IRowsGroupWriter<TData>
{
    private readonly TData _sourceObject;
    private readonly IRowSetter _rowSetter;

    public RowsGroupWriter(TData sourceObject,
        IRowSetter rowSetter)
    {
        _sourceObject = sourceObject;
        _rowSetter = rowSetter;
    }

    public IRowsGroupWriter<TData> Set<TProperty, TTargetValue>(string headerName,
        Func<TData, TProperty> propSelector,
        Action<ICellWriter, TProperty> writerAction = null,
        int headerIndex = 0)
    {
        var value = propSelector(_sourceObject);
        var cellWriter = _rowSetter.CreaterCellWriter(headerName, headerIndex);
        cellWriter.DownMerge(_rowSetter.Rows).SetValue(value);
        writerAction?.Invoke(cellWriter, value);
        return this;
    }
}
