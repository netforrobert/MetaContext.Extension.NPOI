using System.Collections.Generic;
using System.Linq;

namespace MetaContext.Extension.NPOI.Reader;

public class ReadResult<TTargetObject>
{
    public ReadResult(IEnumerable<TTargetObject> data, 
        int processedCount, 
        int successedCount, 
        IEnumerable<ErrowRowInfo> errowRows)
    {
        Data = data;
        ProcessedCount = processedCount;
        SuccessedCount = successedCount;
        ErrowRows = errowRows?.ToArray();
    }

    public ReadResult(ErrowRowInfo errowRowInfo)
        : this(null, 0, 0, new[] { errowRowInfo })
    { }

    public IEnumerable<TTargetObject> Data { get; private set; }

    public int ProcessedCount { get; private set; }

    public int SuccessedCount { get; private set; }

    public IReadOnlyCollection<ErrowRowInfo> ErrowRows { get; private set; }
}

public record ErrowRowInfo
{
    public ErrowRowInfo(int rowNo, List<string> errMessages)
    {
        RowNo = rowNo;
        ErrMessages = errMessages.AsReadOnly();
    }

    public ErrowRowInfo(int rowNo, string errMessage) 
        : this(rowNo, new List<string> { errMessage })
    { 
    }

    public int RowNo { get; private set; }

    public IReadOnlyCollection<string> ErrMessages { get; private set; }
}
