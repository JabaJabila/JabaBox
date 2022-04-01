namespace JabaBox.Core.Domain.Exceptions;

public class DirectoryException : JabaBoxException
{
    public DirectoryException()
    {
    }

    public DirectoryException(string message) : base(message)
    {
    }
}