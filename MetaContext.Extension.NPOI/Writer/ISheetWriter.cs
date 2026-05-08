using System;
using System.Collections.Generic;

namespace MetaContext.Extension.NPOI.Writer;

public interface ISheetWriter
{
    ISheetWriter CreateHeader(Action<ISheetHeader> action,
        int colStartIndex = 0,
        int rowIndex = 0,
        int rows = 1);

    ISheetWriter Write<TSourceObject>(IEnumerable<TSourceObject> sourceObjects,
        Action<IDataSetter<TSourceObject>> writerAction,
        int startRowIndex = -1);

    ISheetWriter UseDefaultAutoWidthSize(int columnsCount = 0);
}
