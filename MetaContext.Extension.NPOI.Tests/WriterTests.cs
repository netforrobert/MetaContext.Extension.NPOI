using NPOI.SS.Formula.Functions;
using NPOI.XSSF.UserModel;

namespace MetaContext.Extension.NPOI.Tests;

public class WriterTests
{
    [Fact]
    public void Test_MultiLevelHeader()
    {
        XSSFWorkbook sheets = new();
        sheets.CreateSheet()
            .UseSheetWriter()
            .CreateHeader(header =>
            {
                header.Draw(col => col.Draw(cell => cell.RightMerge(3).SetHeaderText("基本信息")))
                .Next(skipCols: 1, cellCols: 8)
                .Draw(col => col.Draw(cell => cell.RightMerge(8).SetHeaderText("2025年绩效")))
                .Next(skipCols: 1, cellCols: 2)
                .Draw(col => col.Draw(cell => cell.RightMerge(2).SetHeaderText("2026年绩效")));
            },
            firstCols: 3)
            .CreateHeader(header =>
            {
                string[] quarters = ["第一季度", "第二季度", "第三季度", "第四季度"];
                header.Draw(col =>
                {
                    col.Draw(cell => cell.DownMerge(2).SetHeaderText("员工ID"))
                    .Move(0, 1, cell => cell.DownMerge(2).SetHeaderText("姓名"))
                    .Move(0, 2, cell => cell.DownMerge(2).SetHeaderText("部门"));
                });

                foreach (var quarter in quarters)
                {
                    header.Next(2).Draw(col =>
                    {
                        col.Draw(cell => cell.RightMerge(2).SetHeaderText(quarter))
                          .Move(1, 0, cell => cell.SetHeaderText("销售额"))
                          .Move(1, 1, cell => cell.SetHeaderText("利润"));
                    });
                }
            },
            rowIndex: 1,
            rows: 2,
            firstCols: 3);
    }
}
