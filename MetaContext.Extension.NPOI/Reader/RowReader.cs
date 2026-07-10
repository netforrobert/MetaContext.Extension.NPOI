using System.Collections.Generic;
using System.Linq;

using MetaContext.Extension.NPOI.ColumnIndex;

using NPOI.POIFS.Crypt;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;

namespace MetaContext.Extension.NPOI.Reader;

internal class RowReader : IRowReader
{
    private readonly IRow _row;
    private readonly ColumnIndices _colIndexs;

    public RowReader(IRow row, ColumnIndices colIndexs)
    {
        _row = row;
        _colIndexs = colIndexs;
    }

    public string Read(string column, int index)
    {
        string key = $"{column}_{index}";
        var colIndex = _colIndexs[key];
        return _row.GetCell(colIndex.ColumnIndex)?.ToString();
    }

    public string Read(int index)
        => _row.GetCell(index)?.ToString();
}
