using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

using MetaContext.Extension.NPOI.ColumIndex;

using NPOI.SS.UserModel;

namespace MetaContext.Extension.NPOI.Writer;

internal class DataWriter<TSourceObject> : IDataWriter<TSourceObject>
{
    private readonly IRowWriter _rowWriter;
    private readonly IPropertyGetterProvider _propertyGetterProvider;
    private readonly TSourceObject _sourceObject;

    public DataWriter(ColumnIndices colIndexs,
        IRow row,
        IPropertyGetterProvider propertyGetterProvider,
        TSourceObject sourceObject)
    {
        _rowWriter = new RowWriter(row, colIndexs);
        _propertyGetterProvider = propertyGetterProvider;
        _sourceObject = sourceObject;
    }

    public IDataWriter<TSourceObject> Write<TProperty, TTargetValue>(Expression<Func<TSourceObject, TProperty>> expression, 
        string headerName,
        Func<TProperty, TTargetValue> convertor = null, 
        int index = -1)
    {
        if (expression.Body is not MemberExpression memberExpression
            || memberExpression.Member is not PropertyInfo property)
            throw new NotSupportedException($"不支持的表达式：{expression}");

        string keyName = $"{typeof(TSourceObject).FullName}-{property.Name}";
        Func<TSourceObject, TProperty> selector = _propertyGetterProvider.GetOrAdd(keyName,
            keyName => expression.Compile());

        var value = selector(_sourceObject);
        convertor ??= x => (TTargetValue)Convert.ChangeType(x, typeof(TTargetValue));
        _rowWriter.Writer(headerName, convertor(value), index);
        return this;
    }
}
