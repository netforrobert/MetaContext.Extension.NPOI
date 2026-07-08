using System;
using System.Collections.Generic;
using System.Linq;

using MetaContext.Extension.NPOI.ColumnIndex;
using MetaContext.Extension.NPOI.Header;

using NPOI.SS.UserModel;

namespace MetaContext.Extension.NPOI.Reader;

public abstract class ObjectsReader<TTargetObject> : IObjectsReader<TTargetObject>
{
    private readonly ReaderErrorMessageProvider _errorMessageProvider = new();
    private readonly TargetObjectVerifier<TTargetObject> _objectVerifier = new();
    private readonly List<Action<IRowVerifier>> _appendedRowValidations = new();
    private readonly List<Action<ITargetObjectVerifier<TTargetObject>>> _appendedObjValidations = new();

    private readonly RowVerifier _rowVerifier;
    private readonly ISheet _sheet;
    private readonly IEnumerable<HeaderInfo> _headers;
    private readonly ColumnIndices _columnIndices;


    protected ObjectsReader(ISheet sheet,
        IEnumerable<HeaderInfo> headers)
    {
        _sheet = sheet;
        _headers = headers;

        int rowIndex = _headers.Select(p => p.RowIndex).Max();
        var cols = _headers.Where(p => p.RowIndex == rowIndex)
            .Select(p => p.HeaderText).ToArray();
        _columnIndices = new(cols);
        _rowVerifier = new(_columnIndices, _errorMessageProvider);
    }

    public ReadResult<TTargetObject> Read()
    {
        throw new NotImplementedException();
    }

    public void UseObjectValidations(Action<ITargetObjectVerifier<TTargetObject>> action)
        => _appendedObjValidations.Add(action);

    public void UseRowValidations(Action<IRowVerifier> action)
        => _appendedRowValidations.Add(action);

    protected virtual void ColumnVerify(IColumnVerifier columnVerifier)
    { 
    }

    protected virtual void ColumnsVerify(IColumnsVerifier rowValidation)
    { 
    }

    protected virtual void ObjectVerify(ITargetObjectVerifier<TTargetObject> objectVerifier)
    { 
    }
}
