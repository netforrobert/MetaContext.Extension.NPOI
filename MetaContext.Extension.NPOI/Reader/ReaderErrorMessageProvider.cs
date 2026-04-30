using System;
using System.Collections.Generic;

namespace MetaContext.Extension.NPOI.Reader;

internal class ReaderErrorMessageProvider : IReaderErrorMessageProvider,
    IReaderErrorMessageConfig
{
    private readonly Dictionary<string, object> _valuePairs;

    public ReaderErrorMessageProvider()
    {
        static List<string> ErrHeaderMsgFactory(IEnumerable<ErrorHeaderItem> errorHeaderItems)
        {
            List<string> strings = new()
            {
                "表头校验失败！"
            };
            foreach (var errorHeaderItem in errorHeaderItems)
                strings.Add($"表头{errorHeaderItem.CoIndex.GetUppercaseLetter()}应为：[{errorHeaderItem.ExpectedHeader}] " +
                    $"，实际为：[{errorHeaderItem.ActualHeader}]");

            return strings;
        }

        _valuePairs = new()
        {
            { nameof(NullHeader), "表头行为空" },
            { nameof(ErrorHeaders), (object)ErrHeaderMsgFactory },
            { nameof(ColumnNotEmpty), "列 [{0}] 不能为空" }
        };
    }

    public IReaderErrorMessageConfig ErrorHeaders(Func<IEnumerable<ErrorHeaderItem>, List<string>> messageFactory)
    {
        _valuePairs[nameof(ErrorHeaders)] = messageFactory;
        return this;
    }

    public IReaderErrorMessageConfig NullHeader(string errMessage)
    {
        _valuePairs [nameof(NullHeader)] = errMessage;
        return this;
    }

    public TValue GetMessageValue<TValue>(string key)
        => (TValue)_valuePairs[key];

    public IReaderErrorMessageConfig ColumnNotEmpty(string errMessage)
    {
        _valuePairs[nameof(ColumnNotEmpty)] = errMessage;
        return this;
    }
}
