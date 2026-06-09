using NPOI.SS.UserModel;

namespace MetaContext.Extension.NPOI.Writer;

public interface IRegionCell
{
    int RowIndex { get; }

    int ColumnIndex { get; }

    int Rows { get; }

    int Columns { get; }

    void SetValue<T>(T value, 
        int rightMerge = 1, 
        int downMerge = 1, 
        ICellStyle cellStyle = null);
}
