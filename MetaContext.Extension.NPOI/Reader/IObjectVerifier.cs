namespace MetaContext.Extension.NPOI.Reader;

public interface IObjectVerifier
{
    bool TryVerify(object targetObj, out string message);
}
