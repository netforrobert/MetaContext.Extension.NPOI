using System;

namespace MetaContext.Extension.NPOI.Writer;

public interface IRegionBlock
{
    IRegionBlock Block(string text, Action<IRegionBlock> action);

    void Col(string text, int colspan = 1, int downMerge = 1);
}
