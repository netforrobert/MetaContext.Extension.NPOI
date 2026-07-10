using MetaContext.Extension.NPOI.Header;

namespace MetaContext.Extension.NPOI.Tests;

internal static class HeaderHelper
{
    public static void DrawHeader(this ISheetHeader header)
    {
        header.Block("基本信息", block =>
        {
            block.Cell("员工ID", downMerge: 2);
            block.Cell("姓名", downMerge: 2);
            block.Cell("部门", downMerge: 2);
            block.Cell("入职日期", downMerge: 2);
        })
        .Block("2025年绩效", block =>
        {
            string[] quarters =
            [
                "第一季度",
                "第二季度",
                "第三季度",
                "第四季度"
            ];
            foreach (string quarter in quarters)
            {
                block.Block(quarter, block1 =>
                {
                    block1.Cell("销售额");
                    block1.Cell("利润");
                });
            }
        })
        .Cell("备注", rightMerge: 3, downMerge: 3);
    }
}
