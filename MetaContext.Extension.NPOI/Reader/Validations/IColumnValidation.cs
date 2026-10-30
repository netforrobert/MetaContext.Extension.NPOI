using System;

namespace MetaContext.Extension.NPOI.Reader.Validations;

public interface IColumnValidation
{
    void ColVerify(string column, Action<IColVerifier> action, int index = 0);
}
