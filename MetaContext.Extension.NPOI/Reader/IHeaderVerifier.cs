using MetaContext.Extension.NPOI.ColumnIndex;

using NPOI.SS.UserModel;

namespace MetaContext.Extension.NPOI.Reader;

public interface IHeaderVerifier
{
    int RowIndex { get; }

    ColumnIndices ColIndices { get; }

    ErrorHeaderInfo Verify(IRow row);
}
