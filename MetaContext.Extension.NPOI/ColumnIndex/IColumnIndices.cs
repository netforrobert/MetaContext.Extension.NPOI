using System.Collections.Generic;

namespace MetaContext.Extension.NPOI.ColumnIndex;

public interface IColumnIndices
{
    int ColumnsCount { get; }
    IEnumerable<HeaderIndex> Indices { get; }
    HeaderIndex GetColIndex(string colName, int relativeIndex = 0);
}