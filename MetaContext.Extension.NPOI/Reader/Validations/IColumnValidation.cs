using System;

namespace MetaContext.Extension.NPOI.Reader.Validations;

public interface IColumnValidation
{
    IColumnValidation ColVerify(string column, Action<IColVerifier> action, int index = 0);

    IColumnValidation ColsVerify(Action<IColumnTaker> takerAction, Action<IColVerifier> action);
}
