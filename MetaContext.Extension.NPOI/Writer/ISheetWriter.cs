using System;
using System.Collections.Generic;

using MetaContext.Extension.NPOI.Header;

using NPOI.SS.UserModel;

namespace MetaContext.Extension.NPOI.Writer;

public interface ISheetWriter
{
    ISheetWriter CreateHeader(Action<ISheetHeader> action,
        int colStartIndex = 0,
        int rowIndex = 0,
        ICellStyle cellStyle = null);

    ISheetWriter Write<TSourceObject>(IEnumerable<TSourceObject> sourceObjects,
        Action<IRowsWriter<TSourceObject>> writerAction,
        int startRowIndex = -1,
        Func<TSourceObject, int> rowsSelector = null);

    ISheetWriter UseDefaultAutoWidthSize(int columnsCount = 0);
}
