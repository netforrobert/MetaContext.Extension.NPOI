using NPOI.SS.Formula.Functions;
using NPOI.XSSF.UserModel;

using MetaContext.Extension.NPOI.Writer;
using MetaContext.Extension.NPOI.Header;

namespace MetaContext.Extension.NPOI.Tests;

public class WriterTests
{
    [Fact]
    public void Test_MultiLevelHeader()
    {
        XSSFWorkbook sheets = new();
        sheets.CreateSheet()
            .UseSheetWriter()
            .CreateHeader(header => DrawHeader(header))
            .UseDefaultAutoWidthSize();

        string fileName = $"sheets/{Guid.NewGuid()}.xlsx";
        sheets.SaveToFile(fileName);
    }

    [Fact]
    public void Test_Write()
    {
        //构建模拟数据
        static IEnumerable<ReportItemInfo> GetReportItems()
        {
            Random rnd = new();

            IEnumerable<QuarterlyData> GetQuarterDatas()
            {
                for (int i = 0; i < 4; i++)
                {
                    yield return new()
                    {
                        Quarter = $"Q{i + 1}",
                        Amount = rnd.Next(1000000, 2000000),
                        Profit = rnd.Next(100000, 300000)
                    };
                }
            }

            for (int i = 0; i < 10; i++)
            {
                yield return new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = $"员工{i+1}",
                    Department = "销售部",
                    StartDate = DateTime.Now.AddDays(-(rnd.Next(100,200))),
                    YearlyDatas =
                    [
                        new YearlyData()
                        { 
                            QuarterlyDatas = [.. GetQuarterDatas()]
                        }
                    ],
                    Remark = $"备注_{i}"
                };
            }
        };

        XSSFWorkbook sheets = new();
        sheets.CreateSheet()
            .UseSheetWriter()
            .CreateHeader(header =>
            {
                //此处封装表头创建
                DrawHeader(header);
            })
            .Write(GetReportItems(), writer =>
            {
                writer.Set("员工ID", x => x.Id)
                .Set("姓名", x => x.Name)
                .Set("部门", x => x.Department)
                .Set("入职日期", x => x.StartDate, x => x.ToString("yyyy-MM-dd")) //自定义格式转换
                .Populate((setter, obj) =>
                {
                    //使用Populate进行复杂写入
                    var quarterlyDatas = obj.YearlyDatas.SelectMany(p => p.QuarterlyDatas).ToArray();
                    for (int i = 0; i < quarterlyDatas.Length; i++)
                    {
                        var quarterlyData = quarterlyDatas[i];
                        setter.Set("销售额", quarterlyData.Amount, i)
                        .Set("利润", quarterlyData.Profit, i);
                    }
                })
                .Set("备注", x => x.Remark);
            })
            .UseDefaultAutoWidthSize();

        string fileName = $"sheets/数据写入-{Guid.NewGuid()}.xlsx";
        sheets.SaveToFile(fileName);
    }

    [Fact]
    public void Test_SingleHeader()
    {
        static IEnumerable<string> GetHeaderCols(int rowNo = 0)
        {
            for (int i = 0; i < 10; i++)
            {
                yield return $"表头{rowNo}_{i}";
            }
        }

        XSSFWorkbook sheets = new();
        sheets.CreateSheet().UseSheetWriter()
            .CreateHeader(GetHeaderCols())
            .CreateHeader(GetHeaderCols(1), rowIndex: 1)
            .CreateHeader(GetHeaderCols(3), rowIndex: 3)
            .UseDefaultAutoWidthSize();

        string fileName = $"sheets/Test_SingleHeader_{Guid.NewGuid()}.xlsx";
        sheets.SaveToFile(fileName);
    }

    private static void DrawHeader(ISheetHeader header)
    {
        header.Block("基本信息", block =>
        {
            block.Cell("员工ID", downMerge: 2);
            block.Cell("姓名", downMerge: 2);
            block.Cell("部门", downMerge: 2);
            block.Cell("入职日期", downMerge: 2);
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
        .Cell("备注", rightMerge: 3, downMerge: 3);
    }
}
