using System;
using System.Collections.Generic;
using System.Text;

namespace MetaContext.Extension.NPOI.Reader.Validations;

internal class ColumnTaker : IColumnTaker
{
    private readonly List<ColKey> _colKeys = new();

    public IEnumerable<ColKey> ColKeys => _colKeys;

    public IColumnTaker Take(string column, int index = 0)
    {
        _colKeys.Add(new(column, index));
        return this;
    }

    public IColumnTaker TakeCols(params string[] columns)
    {
        foreach (var column in columns)
            Take(column, 0);

        return this;
    }

    
}
