namespace MetaContext.Extension.NPOI.Reader;

public class ValidationResult
{
    public ValidationResult(bool isSuccess, params object[] arguments)
    {
        IsSuccess = isSuccess;
        Arguments = arguments;
    }

    public bool IsSuccess { get; private set; }

    public object[] Arguments { get; private set; }
}
