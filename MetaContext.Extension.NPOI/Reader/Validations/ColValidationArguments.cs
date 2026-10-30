namespace MetaContext.Extension.NPOI.Reader.Validations;

public record ColValidationArguments
{
    public ColValidationArguments(ColKey colKey, string value)
    {
        ColKey = colKey;
        Value = value;
    }

    public ColKey ColKey { get; private set; }
    public string Value { get; private set; }
}
