using System;
using System.Collections.Generic;

namespace MetaContext.Extension.NPOI.Reader;

public interface IReaderErrorMessageConfig
{
    IReaderErrorMessageConfig NullHeader(string errMessage);

    IReaderErrorMessageConfig ErrorHeaders(Func<IEnumerable<ErrorHeaderItem>, List<string>> messageFactory);

    IReaderErrorMessageConfig ColumnNotEmpty(string errMessage);
}
