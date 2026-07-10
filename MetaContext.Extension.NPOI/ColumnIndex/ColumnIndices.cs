using System.Collections.Generic;
using System.Linq;

using MetaContext.Extension.NPOI.Header;

namespace MetaContext.Extension.NPOI.ColumnIndex;

public class ColumnIndices : IColumnIndices
{
    private readonly Dictionary<string, HeaderIndex> _colIndexs = new();

    public ColumnIndices(IEnumerable<HeaderInfo> headerInfos)
    {
        var groupHeaders = headerInfos.GetBottomHeaders().GroupBy(p => p.HeaderText);
        foreach (var groupHeader in groupHeaders)
        {
            var headers = groupHeader.OrderBy(p => p.ColumnIndex).ToArray();
            for (int i = 0; i < headers.Length; i++)
            {
                HeaderIndex headerIndex = new(headers[i], i);
                _colIndexs[headerIndex.IndexKey] = headerIndex;
            }
        }
    }

    public int ColumnsCount => _colIndexs.Count;

    public HeaderIndex this[string key] => _colIndexs[key];

    public HeaderIndex GetColIndex(string colName, int relativeIndex)
    {
        string key = $"{colName}_{relativeIndex}";
        if (_colIndexs.ContainsKey(key))
            return _colIndexs[key];

        return null;
    }

    public IEnumerable<HeaderIndex> Indices
        => _colIndexs.Values.OrderBy(p => p.ColumnIndex);
}
