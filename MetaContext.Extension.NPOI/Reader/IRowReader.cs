namespace MetaContext.Extension.NPOI.Reader;

public interface IRowReader
{
    string Read(string column, int index = -1);
}
