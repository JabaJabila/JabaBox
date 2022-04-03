namespace JabaBox.Core.Domain.Exceptions;

public class JabaBoxException : Exception
{
    public JabaBoxException()
    {
    }

    public JabaBoxException(string message) : base(message)
    {
    }
}