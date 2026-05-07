using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

using MetaContext.Extension.NPOI.ColumIndex;

using NPOI.SS.UserModel;

namespace MetaContext.Extension.NPOI.Writer;

internal class DataSetter<TSourceObject> : IDataSetter<TSourceObject>
{
    private readonly IRowSetter _rowWriter;
    private readonly IPropertyGetterProvider _propertyGetterProvider;
    private readonly TSourceObject _sourceObject;

    public DataSetter(ColumnIndices colIndexs,
        IRow row,
        IPropertyGetterProvider propertyGetterProvider,
        TSourceObject sourceObject)
    {
        _rowWriter = new RowSetter(row, colIndexs);
        _propertyGetterProvider = propertyGetterProvider;
        _sourceObject = sourceObject;
    }

    public IDataSetter<TSourceObject> ExtraSet(Action<IRowSetter, TSourceObject> extraAction)
    {
        extraAction.Invoke(_rowWriter, _sourceObject);
        return this;
    }

    public IDataSetter<TSourceObject> Set<TProperty, TTargetValue>(string headerName,
        Expression<Func<TSourceObject, TProperty>> expression, 
        Func<TProperty, TTargetValue> convertor = null, 
        int index = 0)
    {
        if (expression.Body is not MemberExpression memberExpression
            || memberExpression.Member is not PropertyInfo property)
            throw new NotSupportedException($"不支持的表达式：{expression}");

        string keyName = $"{typeof(TSourceObject).FullName}-{property.Name}";
        Func<TSourceObject, TProperty> selector = _propertyGetterProvider.GetOrAdd(keyName,
            keyName => expression.Compile());

        static TTargetValue ConvertToTargetValue(TProperty property)
        {
            if (typeof(TTargetValue) == typeof(TProperty))
                return (TTargetValue)(object)property;

            return (TTargetValue)Convert.ChangeType(property, typeof(TTargetValue));
        }

        var value = selector(_sourceObject);
        convertor ??= ConvertToTargetValue;
        _rowWriter.Set(headerName, convertor(value), index);
        return this;
    }

    public IDataSetter<TSourceObject> Set<TProperty>(string headerName,
        Expression<Func<TSourceObject, TProperty>> expression,
        int index = 0)
        => Set<TProperty, TProperty>(headerName, expression, null, index);
}
