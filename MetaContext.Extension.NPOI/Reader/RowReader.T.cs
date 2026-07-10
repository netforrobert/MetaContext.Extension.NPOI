using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace MetaContext.Extension.NPOI.Reader;

internal class RowReader<TTargetObject> : IRowReader<TTargetObject>
    where TTargetObject : class, new()
{
    private readonly Dictionary<ColKey, Action<TTargetObject, string>> _setActions = new();
    private readonly List<Action<TTargetObject, IRowReader>> _extraActions = new();

    public IRowReader<TTargetObject> ForProperties(Action<TTargetObject, IRowReader> action)
    {
        _extraActions.Add(action);
        return this;
    }

    public IRowReader<TTargetObject> ForProperty<TProperty>(Expression<Func<TTargetObject, TProperty>> expression, 
        string column, 
        int index, 
        Func<string, TProperty> convertor = null, 
        Action<TTargetObject> extraAction = null)
    {
        if (expression.Body is not MemberExpression memberExpression
            || memberExpression.Member is not PropertyInfo property
            || property.GetSetMethod() == null)
            throw new NotSupportedException($"列[{column}]的表达式{expression}不包含设置器");

        convertor ??= x => x.ParseToValue<TProperty>();
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

        foreach (var item in _extraActions)
            item(targetObject, rowReader);

        return targetObject;
    }
}
