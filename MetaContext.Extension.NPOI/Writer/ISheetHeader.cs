using System;
using System.Collections.Generic;

namespace MetaContext.Extension.NPOI.Writer;

public interface ISheetHeader
{
    int RowIndex { get; }

    int StartColIndex { get; }

    int Rows { get; }

    int Columns { get; }

    IEnumerable<string> HeaderTexts { get; }

    ISheetHeader Block(string text, Action<IHeaderBlock> action);

    ISheetHeader Cell(string text, int rightMerge = 1, int downMerge = 1);
}
