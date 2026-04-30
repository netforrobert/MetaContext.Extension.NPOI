namespace MetaContext.Extension.NPOI.Reader;

public interface IReaderErrorMessageProvider
{
    TValue GetMessageValue<TValue>(string key);
}
