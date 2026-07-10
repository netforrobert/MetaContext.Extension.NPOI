using MetaContext.Extension.NPOI.ColumnIndex;

namespace MetaContext.Extension.NPOI.Writer;

public interface IRowSetter
{
    int Rows { get; }

    IColumnIndices ColumnIndices { get; }

    IRowSetter Set<TargetValue>(string columnName, TargetValue value, int index = 0);

    ICellWriter CreaterCellWriter(string columnName, int index = 0, int cols = 1);
}
