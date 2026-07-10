using System;

namespace MetaContext.Extension.NPOI.Reader.Validations;

public interface IColumnsVerifier
{
    /// <summary>
    /// 使用组合验证
    /// </summary>
    /// <param name="verfierfunc">验证委托</param>
    /// <param name="messageFactory">消息委托</param>
    /// <param name="isAbortReading">是否终断读取</param>
    void UseValuesVerify(Func<IRowReader, ValidationResult> verfierfunc,
        Func<IRowReader, object[], string> messageFactory,
        bool isAbortReading = false);
}
