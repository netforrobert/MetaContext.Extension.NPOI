using NPOI.SS.UserModel;

namespace MetaContext.Extension.NPOI.Writer;

public interface IRegionCell
{
    void SetValue<T>(T value, 
        int rightMerge = 1, 
        int downMerge = 1, 
        ICellStyle cellStyle = null);
}
