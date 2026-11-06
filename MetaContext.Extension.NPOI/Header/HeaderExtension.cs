using System;
using System.Collections.Generic;
using System.Linq;

using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace MetaContext.Extension.NPOI.Header;

public static class HeaderExtension
{
    public static IEnumerable<HeaderInfo> GetHeaderInfos(this ISheet sheet,
        Action<ISheetHeader> action,
        int colStartIndex = 0,
        int rowIndex = 0)
    {
        var header = new SheetHeader(sheet, rowIndex, colStartIndex, null);
        action(header);
        return header.Headers;
    }

    public static IEnumerable<HeaderInfo> GenerateHeaders(Action<ISheetHeader> action,
        int colStartIndex = 0,
        int rowIndex = 0)
    {
        var sheet = new XSSFWorkbook().CreateSheet();
        var header = new SheetHeader(sheet, rowIndex, colStartIndex, null);
        action(header);
        return header.Headers;
    }

    public static IEnumerable<HeaderInfo> GetBottomHeaders(this IEnumerable<HeaderInfo> headers)
    {
        var colIndexGroups = headers.GroupBy(p => p.ColumnIndex);
        foreach (var colIndexGroup in colIndexGroups)
        {
            var bottomHeader = colIndexGroup.OrderByDescending(p => p.RowIndex).FirstOrDefault();
            yield return bottomHeader;
        }
    }
}
