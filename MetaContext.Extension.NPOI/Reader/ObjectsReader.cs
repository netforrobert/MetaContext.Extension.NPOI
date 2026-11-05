using System;
using System.Collections.Generic;
using System.Linq;

using MetaContext.Extension.NPOI.ColumnIndex;
using MetaContext.Extension.NPOI.Header;
using MetaContext.Extension.NPOI.Reader.Validations;

using NPOI.SS.UserModel;

namespace MetaContext.Extension.NPOI.Reader;

public abstract class ObjectsReader<TTargetObject> : ObjectsReaderBase<TTargetObject>
    where TTargetObject : class, new()
{
    protected ObjectsReader(ISheet sheet,
        IEnumerable<HeaderInfo> headers)
        : base(sheet, headers)
    {
    }

    protected override TTargetObject ReadTargetObject(IRowReader rowReader)
    {
        var objReader = new RowReader<TTargetObject>();
        ConfigTargetObjectReader(objReader);
        return objReader.Read(rowReader);
    }

    protected abstract void ConfigTargetObjectReader(IRowReader<TTargetObject> rowReader);
}
