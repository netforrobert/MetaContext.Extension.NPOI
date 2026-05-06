using System;

namespace MetaContext.Extension.NPOI.Reader;

public static class ReaderExtension
{
    public static TValue ParseToValue<TValue>(this string text)
    {
        if (string.IsNullOrEmpty(text))
            return default;

        Type type = typeof(TValue);
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

        return (TValue)tagValue;
    }
}
