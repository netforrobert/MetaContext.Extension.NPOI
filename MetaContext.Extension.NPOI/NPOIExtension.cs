using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MetaContext.Extension.NPOI.Reader;
using MetaContext.Extension.NPOI.Writer;

using NPOI.SS.UserModel;

namespace MetaContext.Extension.NPOI;

public static class NPOIExtension
{
    public static ISheetWriter UseSheetWriter(this ISheet sheet)
        => new SheetWriter(sheet);

    public static ISheetReader UseSheetReader(this ISheet sheet,
        Action<IReaderErrorMessageConfig> msgConfigAction = null)
    {
        ReaderErrorMessageProvider provider = new();
        msgConfigAction?.Invoke(provider);
        return new SheetReader(sheet, provider);
    }

    public static string GetUppercaseLetter(this int index)
    {
        char uppercaseLetter = (char)('A' + index);
        return uppercaseLetter.ToString();
    }

    public static ISheetWriter CreateHeader(this ISheetWriter sheetWriter,
        IEnumerable<string> headerCols,
        int colStartIndex = 0,
        int rowIndex = 0,
        ICellStyle cellStyle = null)
        {
            sheetWriter.CreateHeader(header=>
            {
                foreach (var headerCol in headerCols)
                    header.Cell(headerCol);
            },
            colStartIndex,
            rowIndex,
            cellStyle);
            return sheetWriter;
        }
}
