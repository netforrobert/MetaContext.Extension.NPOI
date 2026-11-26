using System;

namespace MetaContext.Extension.NPOI.Reader.Validations;

public interface IRowValidation
{
    /// <summary>
    /// 使用组合验证
    /// </summary>
    /// <param name="verfierfunc">验证委托</param>
    /// <param name="messageFactory">消息委托</param>
    /// <param name="isAbortReading">是否终断读取</param>
    void Verify(Func<IRowReader, ValidationResult> verfierfunc,
        Func<IRowReader, object[], string> messageFactory,
        bool isAbortReading = false);
}
