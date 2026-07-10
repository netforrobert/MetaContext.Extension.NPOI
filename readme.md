#### 简介
这是一个NPOI的扩展包，封装了常用的导入导出功能，可以理解为基于NPOI的DSL语言。

##### 数据写入
NPOI本身已经提供了非常丰富的操作Api，非常的灵活。但也因为灵活，要快速创建和调整表格其实并不容易。本章节将通过多维表头创建和数据写入展示DSL的能力。

- 创建多维表头
```c#
XSSFWorkbook sheets = new();
sheets.CreateSheet().UseSheetWriter()
    .CreateHeader(header => header.Block("基本信息", block =>
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
            "第四季度"v
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
    .Cell("备注", rightMerge: 3, downMerge: 3))
    .UseDefaultAutoWidthSize();

string fileName = $"sheets/{Guid.NewGuid()}.xlsx";
sheets.SaveToFile(fileName);
```
输出表头如图所示：
![多维表头](https://chenglin-oss.oss-cn-shenzhen.aliyuncs.com/git-readme/npoi-1.png)

- 数据写入
```c#
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
```
Excel表格最终如图所示：
![数据写入表格](https://chenglin-oss.oss-cn-shenzhen.aliyuncs.com/git-readme/npoi-2.png)

##### 数据读取
读取Excel表格的数据对于NPOI来说并非难事，但在实际开发过程中，对于导入数据的有效性校验显得异常繁琐。本章节将通过介绍一个读取类的使用，来展示DSL标准化校验并读取数据的能力。

我们先假设下面的业务规则：
- 员工ID长度不能超过50
- 除了备注外，导入数据导入数据的各个列都不能为空
- 利润不能大于销售额
- 部门必须是销售部
- 员工Id不能重复出现

基于上述业务规则，我们创建类：**ReportItemInfoReader**
```c#
using MetaContext.Extension.NPOI.Header;
using MetaContext.Extension.NPOI.Reader;
using MetaContext.Extension.NPOI.Reader.Validations;

using NPOI.SS.UserModel;

namespace MetaContext.Extension.NPOI.Tests;

public class ReportItemInfoReader : ObjectsReader<ReportItemInfo>
{
    private readonly HashSet<string> _employeeIds = new();

    public ReportItemInfoReader(ISheet sheet, IEnumerable<HeaderInfo> headers) 
        : base(sheet, headers)
    {
    }

    protected override void ColumnVerify(IColumnVerifier columnVerifier)
    {
        //单列校验
        columnVerifier.Verify("员工ID", x => x.Length > 50, x => $"员工ID：'{x}' 长度超过了50")
            .Verify("部门", x => x != "销售部", x => $"部门 '{x}' 无效")
            .Verify("入职日期", x => !DateTime.TryParse(x, out _), x => $"入职日期：'{x}' 无效");
        //同名列数据有效性校验
        for (int i = 0; i < 4; i++)
        {
            columnVerifier.Verify("销售额", x => !decimal.TryParse(x, out _), x => $"销售额：'{x}' 无效", i)
                .Verify("利润", x => !decimal.TryParse(x, out _), x => $"利润：'{x}' 无效");
        }
        columnVerifier.NotRequire("备注");
    }

    protected override void ColumnsVerify(IColumnsVerifier columnsVerifier)
    {
        //多列校验
        //校验销售额和利润大小
        columnsVerifier.UseValuesVerify(rowReader =>
        {
            for (int i = 0; i < 4; i++)
            {
                var ammount = rowReader.Read<decimal>("销售额", i);
                var profit = rowReader.Read<decimal>("利润", i);
                bool invalid = ammount <= profit;
                if (invalid)
                    return new(invalid, $"利润 '{profit}' 不能大于销售额 '{ammount}' ");
            }

            return new(false);
        },
        (_, args) => args[0].ToString());
    }

    protected override void TTargetObjectVerify(ITargetObjectVerifier<ReportItemInfo> objectVerifier)
    {
        //对象校验，处理Id重复
        objectVerifier.VerifyObject(obj =>
        {
            if (_employeeIds.Contains(obj.Id))
                return new(true, $"员工ID '{obj.Id}' 出现重复");

            _employeeIds.Add(obj.Id);
            return new(false);
        },
        (_, args) => args[0].ToString());
    }

    protected override void ConfigTargetObjectReader(IRowReader<ReportItemInfo> rowReader)
    {
        rowReader.ForProperty(p => p.Id, "员工ID")
            .ForProperty(p => p.Name, "姓名")
            .ForProperty(p => p.Department, "部门")
            .ForProperty(p => p.StartDate, "入职日期", 0, x => DateTime.Parse(x))
            .ForProperties((obj, reader) =>
            {
                //处理复杂读取
                List<QuarterlyData> quarterlyDatas = [];
                for (int i = 0; i < 4; i++)
                {
                    quarterlyDatas.Add(new()
                    {
                        Quarter = $"Q{i + 1}",
                        Amount = reader.Read<decimal>("销售额", i),
                        Profit = reader.Read<decimal>("利润", i)
                    });
                }

                obj.YearlyDatas = [
                    new ()
                    {
                        QuarterlyDatas = [.. quarterlyDatas]
                    }
                ];
            })
            .ForProperty(p => p.Remark, "备注");
    }
}

```
然后，我们准备这样一个数据表格：
![数据导入测试.xlsx](https://chenglin-oss.oss-cn-shenzhen.aliyuncs.com/git-readme/npoi-3.png)

编写读取数据的代码：
```c#
//获取表头
var headers = new XSSFWorkbook()
    .CreateSheet()
    .GetHeaderInfos(header => header.DrawHeader());
//读取导入数据
string fileName = $"sheets/数据导入测试.xlsx";
using XSSFWorkbook sheets = new(fileName);
var sheet = sheets.GetSheetAt(0);
ReportItemInfoReader reader = new(sheet, headers);
var readResult = reader.Read();
//string json = JsonConvert.SerializeObject(readResult);
Assert.Equal(3, readResult.ErrowRows.Count);
```

在省略读取的数据之后，我们会得到这样一个输出，用json格式表示：
```json
{
    "Data": [
        //节省篇幅，此处省去正常数据
    ],
    "ProcessedCount": 4,
    "SuccessedCount": 1,
    "ErrowRows": [
        {
            "RowNo": 4,
            "ErrMessages": [
                "部门 '技术部' 无效",
                "入职日期：'日期' 无效",
                "销售额：'14万元' 无效",
                "利润：'1.3万' 无效"
            ],
            "IsAbortReading": false
        },
        {
            "RowNo": 5,
            "ErrMessages": [
                "利润 '1936650' 不能大于销售额 '108374' "
            ],
            "IsAbortReading": false
        },
        {
            "RowNo": 7,
            "ErrMessages": [
                "员工ID 'ID3' 出现重复"
            ],
            "IsAbortReading": false
        }
    ]
}
```