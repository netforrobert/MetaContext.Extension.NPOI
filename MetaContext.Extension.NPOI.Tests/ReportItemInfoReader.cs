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
