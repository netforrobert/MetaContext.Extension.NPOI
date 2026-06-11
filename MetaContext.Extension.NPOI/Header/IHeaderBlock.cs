using System;
using System.Collections.Generic;

using NPOI.SS.UserModel;

namespace MetaContext.Extension.NPOI.Header;

public interface IHeaderBlock
{
    int RowIndex { get; }

    int ColumnIndex { get; }

    int Rows { get; }

    int Columns { get; }

    IHeaderCell Title { get; }

    ICollection<IHeaderCell> Cells { get; }

    ICollection<IHeaderBlock> Blocks { get; }

    void Block(string text, 
        Action<IHeaderBlock> action,
        ICellStyle cellStyle = null);

    void Cell(string value, 
        int rightMerge = 1, 
        int downMerge = 1,
        ICellStyle cellStyle = null);
}
