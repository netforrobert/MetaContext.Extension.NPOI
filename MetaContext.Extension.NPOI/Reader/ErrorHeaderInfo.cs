using System.Collections.Generic;
using System.Linq;

namespace MetaContext.Extension.NPOI.Reader;

public class ErrorHeaderInfo
{
    public ErrorHeaderInfo(int rowNo, IEnumerable<ErrorHeaderItem> errorItems)
    {
        RowNo = rowNo;
        ErrorItems = errorItems.ToList().AsReadOnly();
    }

    public int RowNo { get; private set; }

    public IReadOnlyCollection<ErrorHeaderItem> ErrorItems { get; private set; }

    public bool IsError => ErrorItems.Count > 0;
}


public record ErrorHeaderItem
{
    public ErrorHeaderItem(int coIndex,
        string expectedHeader, 
        string actualHeader)
    {
        CoIndex = coIndex;
        ExpectedHeader = expectedHeader;
        ActualHeader = actualHeader;
    }

    public int CoIndex { get; private set; }

    public string ExpectedHeader { get; private set; }

    public string ActualHeader { get; private set; }
}