namespace MetaContext.Extension.NPOI.Tests;

public class ReportItemInfo
{
    public string Id { get; set; }

    public string Name { get; set; }

    public string Department { get; set; }

    public DateTime StartDate { get; set; }

    public YearlyData[] YearlyDatas { get; set; }

    public string Remark { get; set; }
}


public class YearlyData
{
    public string Year { get; set; }

    public QuarterlyData[] QuarterlyDatas { get; set; }
}

public record QuarterlyData
{
    public string Quarter { get; set; }

    public decimal Amount { get; set; }

    public decimal Profit { get; set; }
}