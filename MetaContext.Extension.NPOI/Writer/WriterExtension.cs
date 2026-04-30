using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using MetaContext.Extension.NPOI.ColumIndex;

using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;

namespace MetaContext.Extension.NPOI.Writer;

public static class WriterExtension
{
    /// <summary>
    /// 自适应列框宽度。规避中文宽度问题
    /// </summary>
    /// <param name="sheet">sheet</param>
    /// <param name="colsLength">colsLength</param>
    public static void UseDefaultAutoWidthSize(this ISheet sheet, int colsLength)
    {
        static int GetTextLength(string text)
        {
            static bool IsChinese(char c)
                => char.IsLetter(c) && c >= '\u4e00' && c <= '\u9fa5';

            string normalText = string.Join("", text.Where(p => !IsChinese(p)));
            string chineseText = string.Join("", text.Where(p => IsChinese(p)));
            return (int)(Encoding.UTF8.GetBytes(normalText).Length * 1.1)
                + (int)(Encoding.UTF8.GetBytes(chineseText).Length * 1.2);
        }

        //宽度自适应
        for (int i = 0; i < colsLength; i++)
        {
            int columnWidth = (int)sheet.GetColumnWidth(i) / 256;
            for (int rowNum = 0; rowNum <= sheet.LastRowNum; rowNum++)
            {
                IRow currentRow = sheet.GetRow(rowNum);
                if (currentRow == null)
                {
                    continue;
                }

                if (currentRow != null)
                {
                    ICell currentCell = currentRow.GetCell(i);
                    if (currentCell != null)
                    {
                        int length = GetTextLength(currentCell.ToString().Trim());
                        columnWidth = Math.Max(columnWidth, length);
                    }
                }
            }

            sheet.AutoSizeColumn(i);
            var colWidth = Math.Min(columnWidth, 255) * 256;
            sheet.SetColumnWidth(i, colWidth);
        }
    }

    public static void SetNormalBorder(this ICellStyle style)
    {
        // 设置边框样式
        style.BorderBottom = BorderStyle.Thin;
        style.BorderLeft = BorderStyle.Thin;
        style.BorderRight = BorderStyle.Thin;
        style.BorderTop = BorderStyle.Thin;

        // 设置边框颜色
        style.BottomBorderColor = IndexedColors.Black.Index;
        style.LeftBorderColor = IndexedColors.Black.Index;
        style.RightBorderColor = IndexedColors.Black.Index;
        style.TopBorderColor = IndexedColors.Black.Index;
    }

    public static BytesContent ToBytesContent(this IWorkbook workbook,
        string fileName)
    {
        // 使用 MemoryStream 将工作簿保存为二进制数组
        using var memoryStream = new MemoryStream();
        workbook.Write(memoryStream);
        byte[] content = memoryStream.ToArray();

        return new BytesContent(fileName,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            content);
    }
}
