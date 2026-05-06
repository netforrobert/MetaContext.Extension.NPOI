using System.Collections.Generic;

namespace MetaContext.Extension.NPOI.ColumIndex;

public interface IColumnIndices
{
    int ColumnsCount { get; }
    IEnumerable<ColumnIndex> Indices { get; }
    ColumnIndex GetColIndex(string colName, int relativeIndex = 0);
}