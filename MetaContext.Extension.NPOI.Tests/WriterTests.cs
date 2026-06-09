using NPOI.SS.Formula.Functions;
using NPOI.XSSF.UserModel;

using MetaContext.Extension.NPOI.Writer;

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
                header.Block("基本信息", block =>
                {
                    block.Cell("员工ID", downMerge: 2);
                    block.Cell("姓名", downMerge: 2);
                    block.Cell("部门", downMerge: 2);
                })
                .Block("2025年绩效", block =>
                {
                    string[] quarters =
                    [
                        "第一季度",
                        "第二季度",
                        "第三季度",
                        "第四季度"
                    ];
                    foreach (string quarter in quarters)
                    {
                        block.Block(quarter, block1 =>
                        {
                            block1.Cell("销售额");
                            block1.Cell("利润");
                        });
                    }
                })
                .Block(block => block.Cell("备注", rightMerge: 3, downMerge: 3));
            });

        string fileName = $"sheets/{Guid.NewGuid()}.xlsx";
        sheets.SaveToFile(fileName);
    }
}
