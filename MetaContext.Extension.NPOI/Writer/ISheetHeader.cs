using System;
using System.Collections.Generic;

namespace MetaContext.Extension.NPOI.Writer;

public interface ISheetHeader
{
    int RowIndex { get; }

    int StartColIndex { get; }

    int Rows { get; }

    int Cols { get; }

    IEnumerable<string> HeaderTexts { get; }

    ISheetHeader Draw(Action<IRegionHeader> action);

    ISheetHeader Next(int skipCols = 1, int cellCols = 1);
}
