using MetaContext.Extension.NPOI.Header;

namespace MetaContext.Extension.NPOI.ColumnIndex;

public record HeaderIndex : HeaderInfo
{
    public HeaderIndex(HeaderInfo header, int relativeIndex)
        : base(header)
    {
        IndexKey = $"{header.HeaderText}_{relativeIndex}";
    }

    public string IndexKey { get; private set; }
}