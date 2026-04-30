namespace MetaContext.Extension.NPOI.Writer;

public interface IRowWriter
{
    IRowWriter Writer<TargetValue>(string columnName, TargetValue value, int index = -1);
}
