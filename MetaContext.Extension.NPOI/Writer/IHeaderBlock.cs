using System;
using System.Collections.Generic;

namespace MetaContext.Extension.NPOI.Writer;

public interface IHeaderBlock
{
    int RowIndex { get; }

    int ColumnIndex { get; }

    int Rows { get; }

    int Columns { get; }

    IHeaderCell Title { get; }

    ICollection<IHeaderCell> Cells { get; }

    ICollection<IHeaderBlock> Blocks { get; }

    void Block(string text, Action<IHeaderBlock> action);

    void Block(Action<IHeaderBlock> action);

    void Cell(string value, int rightMerge = 1, int downMerge = 1);
}
