using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace MetaContext.Extension.NPOI.Writer;

internal class RowsWriter<TData> : IRowsWriter<TData>
{
    private readonly IPropertyGetterProvider _propertyGetterProvider;
    private readonly TData _sourceObject;
    private readonly IRowSetter _rowSetter;

    public RowsWriter(int startRowIndex,
        IPropertyGetterProvider propertyGetterProvider,
        TData sourceObject,
        IRowSetter rowSetter)
    {
        StartRowIndex = startRowIndex;
        _propertyGetterProvider = propertyGetterProvider;
        _sourceObject = sourceObject;
        _rowSetter = rowSetter;
    }

    public int Rows { get; private set; }

    public int StartRowIndex { get; private set; }

    public IRowsWriter<TData> ExtraSet(Action<IRowSetter, TData> extraAction)
    {
        extraAction.Invoke(_rowSetter, _sourceObject);
        return this;
    }

    public IRowsWriter<TData> Set<TProperty, TTargetValue>(string headerName,
        Expression<Func<TData, TProperty>> expression,
        Func<TProperty, TTargetValue> convertor = null,
        int headerIndex = 0)
    {
        if (expression.Body is not MemberExpression memberExpression
            || memberExpression.Member is not PropertyInfo property)
            throw new NotSupportedException($"不支持的表达式：{expression}");

        string keyName = $"{typeof(TData).FullName}-{property.Name}";
        Func<TData, TProperty> selector = _propertyGetterProvider.GetOrAdd(keyName,
            keyName => expression.Compile());

        static TTargetValue ConvertToTargetValue(TProperty property)
        {
            if (typeof(TTargetValue) == typeof(TProperty))
                return (TTargetValue)(object)property;

            return (TTargetValue)Convert.ChangeType(property, typeof(TTargetValue));
        }

        var value = selector(_sourceObject);
        convertor ??= ConvertToTargetValue;
        _rowSetter.Set(headerName, convertor(value), headerIndex);
        return this;
    }

    public IRowsWriter<TData> Set<TProperty>(string headerName,
        Expression<Func<TData, TProperty>> expression,
        int headerIndex = 0)
        => Set<TProperty, TProperty>(headerName, expression, null, headerIndex);

    public IRowsWriter<TData> SetItems<TItem>(string headerName,
        Expression<Func<TData, IEnumerable<TItem>>> dataSourceExp,
        Expression<Func<TData, int>> rowsSelector,
        Action<IRowsGroupWriter<TData>> grpWriterAction,
        Action<IRowsWriter<TItem>> itemsAction)
    {
        throw new NotImplementedException();
    }

}
