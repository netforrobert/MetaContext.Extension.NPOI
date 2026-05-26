using System.Collections.Generic;

namespace MetaContext.Extension.NPOI.ColumnIndex;

public interface IColumnIndices
{
    int ColumnsCount { get; }
    IEnumerable<ColIndex> Indices { get; }
    ColIndex GetColIndex(string colName, int relativeIndex = 0);
}