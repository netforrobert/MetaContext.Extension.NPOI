namespace MetaContext.Extension.NPOI.Writer;

public class BytesContent
{
    public BytesContent(string fileName, 
        string contentType, 
        byte[] content)
    {
        FileName = fileName;
        ContentType = contentType;
        Content = content;
    }

    public string FileName { get; private set; }

    public string ContentType { get; private set; }

    public byte[] Content { get; private set; }
}
