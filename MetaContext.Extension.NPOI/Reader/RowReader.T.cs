using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace MetaContext.Extension.NPOI.Reader;

internal class RowReader<TTargetObject> : IRowReader<TTargetObject>
    where TTargetObject : class, new()
{
    private readonly Dictionary<ColKey, Action<TTargetObject, string>> _setActions = new();

    public IRowReader<TTargetObject> ForProperty<TProperty>(Expression<Func<TTargetObject, TProperty>> expression, 
        string column, 
        int index, 
        Func<string, TProperty> convertor = null, 
        Action<TTargetObject> extraAction = null)
    {
        static TProperty PropertyConvertor(string text)
        {
            Type type = typeof(TProperty);
            if (type.IsGenericType
                && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                type = type.GetGenericArguments()[0];
            }

            object tagValue = type switch
            {
                Type t when t == typeof(int) => int.Parse(text),
                Type t when t == typeof(long) => long.Parse(text),
                Type t when t == typeof(Guid) => Guid.Parse(text),
                Type t when t == typeof(decimal) => decimal.Parse(text),
                Type t when t.IsEnum => Enum.Parse(t, text),
                Type t when t == typeof(DateTime) => DateTime.Parse(text),
                _ => Convert.ChangeType(text, type)
            };

            return (TProperty)tagValue;
        }

        if (expression.Body is not MemberExpression memberExpression
            || memberExpression.Member is not PropertyInfo property
            || property.GetSetMethod() == null)
            throw new NotSupportedException($"列[{column}]的表达式{expression}不包含设置器");

        convertor ??= PropertyConvertor;
        void setAction(TTargetObject x, string y)
        {
            var value = convertor(y);
            property.SetValue(x, value);
            extraAction?.Invoke(x);
        }
        _setActions[new(column, index)] = setAction;
        return this;
    }

    public TTargetObject Read(IRowReader rowReader)
    {
        TTargetObject targetObject = new();
        foreach (var item in _setActions)
        {
            string value = rowReader.Read(item.Key.Column, item.Key.Index);
            item.Value(targetObject, value);
        }

        return targetObject;
    }
}
