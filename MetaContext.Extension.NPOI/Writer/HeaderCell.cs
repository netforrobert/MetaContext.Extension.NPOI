using NPOI.SS.UserModel;

namespace MetaContext.Extension.NPOI.Writer;

internal class HeaderCell : RegionCell, IHeaderCell
{
    private string _text;

    public HeaderCell(IRow row, int colIndex) : base(row, colIndex)
    {
    }

    public string HeaderText => _text;

    public void Text(string text, 
        int rightMerge = 1, 
        int downMerge = 1, 
        ICellStyle cellStyle = null)
    {
        SetValue(text, rightMerge, downMerge, cellStyle);
        _text = text;
    }
}
