using System;

namespace MetaContext.Extension.NPOI.Writer;

public interface IRegionHeader
{
    int Rows { get; }

    int Cols { get; }

    int RowIndex { get; }

    int ColumnIndex { get; }

    IRegionHeader Draw(Action<ICellHeader> action);

    IRegionHeader Move(int rowOffset, int colOffset, Action<ICellHeader> action);
}
