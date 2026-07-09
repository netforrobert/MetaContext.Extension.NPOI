namespace MetaContext.Extension.NPOI.Reader.Validations;

public interface IObjectVerifier
{
    (bool isInvalid, bool isAbortReading) TryVerify(object targetObj, out string message);
}
