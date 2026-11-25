using System;

using MetaContext.Extension.NPOI.ColumnIndex;

namespace MetaContext.Extension.NPOI.Writer;

public interface IRowSetter
{
    int Rows { get; }

    IColumnIndices ColumnIndices { get; }

    /// <summary>
    /// 设置目标值
    /// </summary>
    /// <typeparam name="TargetValue">目标值类型</typeparam>
    /// <param name="columnName">列名</param>
    /// <param name="value"></param>
    /// <param name="index">下标。默认为0，用来处理同名列</param>
    /// <param name="setIf">为true执行set。默认为true</param>
    /// <returns></returns>
    IRowSetter Set<TTargetValue>(string columnName,
        TTargetValue value, 
        int index = 0, 
        Func<TTargetValue, bool> setIf = null);

    ICellWriter CreaterCellWriter(string columnName, int index = 0, int cols = 1);
}
