using System;

using NPOI.SS.UserModel;

namespace MetaContext.Extension.NPOI.Reader.Validations;

public interface IRowVerifier
{
    IRowVerifier VerifyColumns(Action<IColumnValidation> action);

    IRowVerifier VerifyRow(Action<IColumnsVerifier> action);
}
