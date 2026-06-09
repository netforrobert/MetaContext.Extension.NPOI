using NPOI.SS.UserModel;

namespace MetaContext.Extension.NPOI.Writer;

public interface IHeaderCell : IRegionCell
{
    string HeaderText { get; }

    void Text(string text, 
        int rightMerge = 1,
        int downMerge = 1,
        ICellStyle cellStyle = null);
}
