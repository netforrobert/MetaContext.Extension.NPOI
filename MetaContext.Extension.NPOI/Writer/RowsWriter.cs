using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

using NPOI.SS.UserModel;

namespace MetaContext.Extension.NPOI.Writer;

internal class RowsWriter<TData> : IRowsWriter<TData>
{
    private readonly TData _sourceObject;
    private readonly IRowSetter _rowSetter;
    private readonly ISheet _sheet;

    public RowsWriter(int startRowIndex,
        TData sourceObject,
        IRowSetter rowSetter,
        ISheet sheet)
    {
        StartRowIndex = startRowIndex;
        _sourceObject = sourceObject;
        _rowSetter = rowSetter;
        _sheet = sheet;
    }

    public int Rows => _rowSetter.Rows;

    public int StartRowIndex { get; private set; }

    public IRowsWriter<TData> Populate(Action<IRowSetter, TData> extraAction)
    {
        extraAction.Invoke(_rowSetter, _sourceObject);
        return this;
    }

    public IRowsWriter<TData> Set<TProperty, TTargetValue>(string headerName,
        Func<TData, TProperty> propertyFactory,
        Func<TProperty, TTargetValue> convertor = null,
        int headerIndex = 0)
    {
        static TTargetValue ConvertToTargetValue(TProperty property)
        {
            if (typeof(TTargetValue) == typeof(TProperty))
                return (TTargetValue)(object)property;

            return (TTargetValue)Convert.ChangeType(property, typeof(TTargetValue));
        }

        var value = propertyFactory(_sourceObject);
        convertor ??= ConvertToTargetValue;
        _rowSetter.Set(headerName, convertor(value), headerIndex);
        return this;
    }

    public IRowsWriter<TData> Set<TProperty>(string headerName,
        Func<TData, TProperty> propertyFactory,
        int headerIndex = 0)
        => Set<TProperty, TProperty>(headerName, propertyFactory, null, headerIndex);

    public IRowsWriter<TData> SetItems<TItem>(Func<TData, IEnumerable<TItem>> itemsSelector,
        Func<TData, int> rowsSelector,
        Action<IRowsGroupWriter<TData>> grpWriterAction,
        Action<IRowsWriter<TItem>> itemsAction,
        Func<TItem, int> itemRowsSelector)
    {
        int rows = rowsSelector(_sourceObject);
        var items = itemsSelector(_sourceObject);
        IRowsGroupWriter<TData> grpWriter = new RowsGroupWriter<TData>(_sourceObject, _rowSetter);
        grpWriterAction(grpWriter);

        int rowIndex = StartRowIndex;
        foreach (var item in items)
        {
            var itemRow = _sheet.GetRow(rowIndex) ?? _sheet.CreateRow(rowIndex);
            itemRowsSelector ??= x => 1;
            int itemRows = itemRowsSelector(item);
            var itemRowSetter = new RowSetter(itemRow, _rowSetter.ColumnIndices, itemRows);
            IRowsWriter<TItem> rowsWriter = new RowsWriter<TItem>(StartRowIndex,
                item,
                itemRowSetter,
                _sheet);
            itemsAction(rowsWriter);
            rowIndex += rowsWriter.Rows;
        }

        return this;
    }

}
