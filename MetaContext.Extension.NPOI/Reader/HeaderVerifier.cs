using System.Collections.Generic;

using MetaContext.Extension.NPOI.ColumnIndex;

using NPOI.SS.UserModel;

namespace MetaContext.Extension.NPOI.Reader;

internal class HeaderVerifier : IHeaderVerifier
{
    public HeaderVerifier(int rowIndex, ColumnIndices colIndexs)
    {
        RowIndex = rowIndex;
        ColIndices = colIndexs;
    }

    public int RowIndex { get; private set; }

    public ColumnIndices ColIndices { get; private set; }

    public ErrorHeaderInfo Verify(IRow headerRow)
    {
        List<ErrorHeaderItem> errorHeaders = new();
        foreach (var colIndex in ColIndices.Indices)
        {
            string actualHeader = headerRow.GetCell(colIndex.ColumnIndex)?.ToString() ?? "";
            if (actualHeader != colIndex.HeaderText)
                errorHeaders.Add(new(colIndex.ColumnIndex, colIndex.HeaderText, actualHeader));
        }

        return new ErrorHeaderInfo(headerRow.RowNum, errorHeaders);
    }
}
