using System;

namespace MetaContext.Extension.NPOI.Writer;

public interface IRegionBlock
{
    int StartRowIndex { get; }

    int StartColIndex { get; }

    int Rows { get; }

    int Cols { get; }

    void Block(string text, Action<IRegionBlock> action);

    void Col(string text, int rightMerge = 1, int downMerge = 1);
}
