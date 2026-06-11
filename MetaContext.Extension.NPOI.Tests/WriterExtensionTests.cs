using NPOI.XSSF.UserModel;

using MetaContext.Extension.NPOI.Writer;

namespace MetaContext.Extension.NPOI.Tests;

public class WriterExtensionTests
{
    [Fact]
    public void Test_CellSetValue()
    {
        var sheets = new XSSFWorkbook();
        var row = sheets.CreateSheet().CreateRow(0);
        row.CreateCell(0).SetTargetValue(100.20m);
        row.CreateCell(1).SetTargetValue((decimal?)200.20m);
        row.CreateCell(2).SetTargetValue((decimal?)null);
        row.CreateCell(3).SetTargetValue(true);
        row.CreateCell(4).SetTargetValue(DateTime.Now);
        row.CreateCell(5).SetTargetValue((short)1);
        row.CreateCell(6).SetTargetValue(100000000000000000L);
        row.CreateCell(7).SetTargetValue(100);
        row.CreateCell(8).SetTargetValue(Guid.NewGuid());
        row.CreateCell(9).SetTargetValue("测试文本");
        row.CreateCell(10).SetTargetValue(true.ToString());
        sheets.SaveToFile($"sheets/Test_CellSetValue_{Guid.NewGuid()}.xlsx");
    }
}
