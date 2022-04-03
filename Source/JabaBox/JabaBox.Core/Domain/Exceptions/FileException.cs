namespace JabaBox.Core.Domain.Exceptions;

public class FileException : JabaBoxException
{
    public FileException()
    {
    }

    public FileException(string message) : base(message)
    {
    }
}