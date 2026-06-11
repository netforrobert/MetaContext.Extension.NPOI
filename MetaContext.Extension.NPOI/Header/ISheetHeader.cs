using System;
using System.Collections.Generic;

using NPOI.SS.UserModel;

namespace MetaContext.Extension.NPOI.Header;

public interface ISheetHeader
{
    int RowIndex { get; }

    int StartColIndex { get; }

    int Rows { get; }

    int Columns { get; }

    IEnumerable<string> HeaderTexts { get; }

    IEnumerable<IHeaderInfo> Headers { get; }

    ISheetHeader Block(string text, 
        Action<IHeaderBlock> action, 
        ICellStyle cellStyle = null);

    ISheetHeader Cell(string text, 
        int rightMerge = 1, 
        int downMerge = 1, 
        ICellStyle cellStyle = null);

    ICellStyle CreateDefaultHeaderStyle();
}
