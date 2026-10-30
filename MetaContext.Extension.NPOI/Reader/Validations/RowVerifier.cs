using System;
using System.Collections.Generic;
using System.Linq;

using MetaContext.Extension.NPOI.ColumnIndex;

using NPOI.SS.Formula.PTG;
using NPOI.SS.UserModel;

namespace MetaContext.Extension.NPOI.Reader.Validations;

internal class RowVerifier : IRowVerifier
{
    private readonly List<ColumnsVerifier> _rowValidations = new();
    private readonly ColumnIndices _columnIndices;
    private readonly ColumnVerifier _columnVerifier;
    private readonly ColumnValidation _columnValidation;

    public RowVerifier(ColumnIndices columnIndices,
        IReaderErrorMessageProvider messageProvider)
    {
        _columnIndices = columnIndices;
        _columnVerifier = new(columnIndices, messageProvider);
        _columnValidation = new(messageProvider);
    }

    public IRowVerifier VerifyColumn(Action<IColumnVerifier> action)
    {
        action(_columnVerifier);
        return this;
    }

    public ErrowRowInfo RunVerify(IRow row)
    {
        var rowReader = new RowReader(row, _columnIndices);
        var errorMessages = _columnVerifier.RunVerify(row).ToList();
        var invalidMsg = _columnValidation.GetInvalidMessage(rowReader).ToArray();
        errorMessages.AddRange(invalidMsg);
        if (errorMessages.Count > 0) 
            return new ErrowRowInfo(row.RowNum + 1, errorMessages.ToList());

        //行验证
        bool isBreakLoop = false;
        foreach (var rowValidation in _rowValidations)
        {
            (bool isInvalid, isBreakLoop) = rowValidation.IsErrorRow(rowReader, out string errMessage);
            if (isInvalid)
            {
                errorMessages.Add(errMessage);
                break;
            }
        }

        return new ErrowRowInfo(row.RowNum + 1, errorMessages, isBreakLoop);
    }

    public IRowVerifier VerifyRow(Action<IColumnsVerifier> action)
    {
        ColumnsVerifier validation = new();
        action(validation);
        _rowValidations.Add(validation);
        return this;
    }

    public IRowVerifier VerifyColumns(Action<IColumnValidation> action)
    {
        action(_columnValidation);
        return this;
    }
}
