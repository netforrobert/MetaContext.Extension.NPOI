namespace MetaContext.Extension.NPOI.Reader;

public interface IRowReader
{
    string Read(string column, int index = 0);

    string Read(int index);
}
