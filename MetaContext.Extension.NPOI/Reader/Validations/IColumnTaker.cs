namespace MetaContext.Extension.NPOI.Reader.Validations;

public interface IColumnTaker
{
    IColumnTaker Take(string column, int index = 0);

    IColumnTaker TakeCols(params string[] columns);
}
