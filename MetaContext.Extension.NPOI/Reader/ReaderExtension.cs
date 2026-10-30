using System;

using MetaContext.Extension.NPOI.Reader.Validations;

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

    public static TValue Read<TValue>(this IRowReader reader, 
        string column, 
        int index = 0)
    {
        string text = reader.Read(column, index);
        return text.ParseToValue<TValue>();
    }

    public static TValue Read<TValue>(this IRowReader reader,
        ColKey colKey)
    {
        string text = reader.Read(colKey.Column, colKey.Index);
        return text.ParseToValue<TValue>();
    }

    public static IRowVerifier VerifyColumn(this IRowVerifier rowVerifier,
        string column,
        Func<string, bool> verifyFunc,
        Func<string, string> errTextFunc,
        int index = 0)
        => rowVerifier.VerifyColumn(col => col.Verify(column, verifyFunc, errTextFunc, index));

    public static IRowVerifier NotRequireColumn(this IRowVerifier rowVerifier,
        string columnm,
        int index = 0)
        => rowVerifier.VerifyColumn(col => col.NotRequire(columnm, index));

    [Obsolete("已废弃")]
    public static IColumnVerifier VerifyColumns(this IColumnVerifier columnVerifier, 
        Func<string, bool> verifyFunc,
        Func<string, string, string> errTextFunc,
        params string[] columns)
    {
        foreach (var column in columns)
            columnVerifier.Verify(column, verifyFunc, x => errTextFunc(x, column));

        return columnVerifier;
    }

    [Obsolete("已废弃")]
    public static IColumnVerifier NotRequireColumns(this IColumnVerifier columnVerifier,
        params string[] columns)
    {
        foreach (var column in columns)
            columnVerifier.NotRequire(column);

        return columnVerifier;
    }

    public static void ColsVerify(this IColumnValidation validation,
        Action<IColVerifier> action,
        int index = 0,
        params string[] columns)
    {
        foreach (var column in columns)
        {
            validation.ColVerify(column, action, index);
        }
    }
}
