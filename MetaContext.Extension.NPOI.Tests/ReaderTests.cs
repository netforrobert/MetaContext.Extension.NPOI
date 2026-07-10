using NPOI.XSSF.UserModel;
using MetaContext.Extension.NPOI.Header;

using Newtonsoft.Json;

namespace MetaContext.Extension.NPOI.Tests;

public class ReaderTests
{
    [Fact]
    public void Test_Read()
    {
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
        string json = JsonConvert.SerializeObject(readResult);
        Assert.Equal(3, readResult.ErrowRows.Count);
    }
}
