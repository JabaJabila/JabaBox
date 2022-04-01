using JabaBox.Core.Domain.Entities;

namespace JabaBox.Core.Domain.Exceptions;

public class AccountInfoException : JabaBoxException
{
    public AccountInfoException()
    {
    }

    public AccountInfoException(string message) : base(message)
    {
    }
}