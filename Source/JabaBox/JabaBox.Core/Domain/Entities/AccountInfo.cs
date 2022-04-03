using JabaBox.Core.Domain.Exceptions;

namespace JabaBox.Core.Domain.Entities;

public class AccountInfo
{
    public AccountInfo(string login, string password, int gigabytesAvailable)
    {
        if (string.IsNullOrWhiteSpace(login))
            throw new AccountInfoException("Impossible to set null or empty string as a login");
        
        if (string.IsNullOrWhiteSpace(password))
            throw new AccountInfoException("Impossible to set null or empty string as a password");

        if (gigabytesAvailable <= 0)
            throw new AccountInfoException("Impossible to set limit for <= 0 gigabytes");
        
        Login = login;
        Password = password;
        GigabytesAvailable = gigabytesAvailable;
    }

    private AccountInfo()
    {
    }
    
    public Guid Id { get; private init; }
    public string Login { get; private init; }
    public string Password { get; set; }
    public int GigabytesAvailable { get; set; }
}