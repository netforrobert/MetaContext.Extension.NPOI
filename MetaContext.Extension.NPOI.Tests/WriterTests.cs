using NPOI.SS.Formula.Functions;
using NPOI.XSSF.UserModel;

using static NPOI.HSSF.Util.HSSFColor;

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
                    block.Col("员工ID", downMerge: 2);
                    block.Col("姓名", downMerge: 2);
                    block.Col("部门", downMerge: 2);
                })
                .Block("2025年绩效", block =>
                {
                    string[] quarters = ["第一季度", "第二季度", "第三季度", "第四季度"];
                    foreach (string quarter in quarters)
                    {
                        block.Block(quarter, block1 =>
                        {
                            block1.Col("销售额");
                            block1.Col("利润");
                        });
                    }
                })
                .Col("备注", colspan: 3, downMerge: 3);
            },
            rows: 3,
            firstCols: 3);
    }
}
