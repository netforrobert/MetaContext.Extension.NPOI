using NPOI.SS.UserModel;

namespace MetaContext.Extension.NPOI.Header;

public static class HeaderExtension
{
    public static ISheetHeader CreateHeader(this ISheet sheet,
        int colStartIndex = 0,
        int rowIndex = 0,
        ICellStyle cellStyle = null)
        => new SheetHeader(sheet, rowIndex, colStartIndex, cellStyle);
}
