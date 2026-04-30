using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetaContext.Extension.NPOI.ColumIndex;

public class ColumnIndices : IColumnIndices
{
    private readonly Dictionary<string, ColumnIndex> _colIndexs = new();
    private readonly Dictionary<string, int> _relativeIndexs = new();

    public ColumnIndices(string[] columns,
        int startIndex = 0)
    {
        IEnumerable<int[]> GetMergeColIndexs()
        {
            for (int i = 0; i < columns.Length; i++)
            {
                int index = startIndex + i;
                var header = columns[i];
                if (string.IsNullOrEmpty(header))
                    continue;

                List<int> indexs = new()
                {
                    index
                };
                for (int j = i + 1; j < columns.Length; j++)
                {
                    var emptyHeader = columns[j];
                    if (string.IsNullOrEmpty(emptyHeader))
                    {
                        indexs.Add(startIndex + j);
                    }
                    else
                    {
                        break;
                    }
                }

                if (indexs.Count > 1)
                {
                    yield return indexs.ToArray();
                }
            }
        }

        var mergedIndexs = GetMergeColIndexs().ToDictionary(p => p[0]);
        Dictionary<string, int> cols = new();
        for (int i = 0; i < columns.Length; i++)
        {
            string col = columns[i];
            if (string.IsNullOrEmpty(col))
                continue;

            int relativeIndex = 0;
            if (cols.ContainsKey(col))
            {
                relativeIndex = cols[col] + 1;
                cols[col] = relativeIndex;
            }
            else
                cols[col] = relativeIndex;

            string key = $"{col}_{relativeIndex}";
            int index = i + startIndex;

            if (mergedIndexs.ContainsKey(index))
            {
                var indexs = mergedIndexs[index];
                _colIndexs[key] = new ColumnIndex(indexs[0], indexs[1], col, relativeIndex);
            }
            else
                _colIndexs[key] = new ColumnIndex(index, index, col, relativeIndex);
        }
    }

    public int ColumnsCount => _colIndexs.Count;

    public ColumnIndex this[string key] => _colIndexs[key];

    public ColumnIndex GetColIndex(string colName, int relativeIndex = -1)
    {
        if (relativeIndex < 0)
        {
            if (!_relativeIndexs.ContainsKey(colName))
                _relativeIndexs[colName] = 0;
            else
            {
                var valIndex = _relativeIndexs[colName];
                _relativeIndexs[colName] = valIndex + 1;
            }

            relativeIndex = _relativeIndexs[colName];
        }

        string key = $"{colName}_{relativeIndex}";
        if (_colIndexs.ContainsKey(key))
            return _colIndexs[key];

        return null;
    }

    public IEnumerable<ColumnIndex> Indices
        => _colIndexs.Values.OrderBy(p => p.StartIndex);
}
