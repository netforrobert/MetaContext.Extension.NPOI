namespace MetaContext.Extension.NPOI.Writer;

public interface IRowSetter
{
    IRowSetter Set<TargetValue>(string columnName, TargetValue value, int index = 0);
}
