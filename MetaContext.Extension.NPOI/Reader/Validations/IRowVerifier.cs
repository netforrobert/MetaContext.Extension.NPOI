using System;

using NPOI.SS.UserModel;

namespace MetaContext.Extension.NPOI.Reader.Validations;

public interface IRowVerifier
{
    IRowVerifier VerifyColumn(Action<IColumnVerifier> action);

    IRowVerifier VerifyRow(Action<IColumnsVerifier> action);
}
