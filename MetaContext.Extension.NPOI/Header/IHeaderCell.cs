using MetaContext.Extension.NPOI.Writer;

using NPOI.SS.UserModel;

namespace MetaContext.Extension.NPOI.Header;

public interface IHeaderCell : IRegionCell
{
    string HeaderText { get; }

    void Text(string text, 
        int rightMerge = 1,
        int downMerge = 1,
        ICellStyle cellStyle = null);
}
