using System;
using System.Collections.Generic;

namespace MetaContext.Extension.NPOI.Writer;

public interface ISheetWriter
{
    ISheetWriter CreateHeader(string[] headers,
        int colStartIndex = 0,
        int rowIndex = 0);

    ISheetWriter Write<TSourceObject>(IEnumerable<TSourceObject> sourceObjects,
        Action<IDataWriter<TSourceObject>> writerAction,
        int startRowIndex = -1);

    ISheetWriter UseDefaultAutoWidthSize(int columnsCount = 0);
}
