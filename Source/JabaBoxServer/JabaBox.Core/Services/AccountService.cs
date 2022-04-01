using JabaBox.Core.Domain.Entities;
using JabaBox.Core.Domain.ServicesAbstractions;

namespace JabaBox.Core.Services;

public class AccountService : IAccountService
{
    public AccountInfo RegisterAccount(string login, string password, int gigabyteAvailable)
    {
        throw new NotImplementedException();
    }

    public AccountInfo ChangePassword(string login, string oldPassword, string newPassword)
    {
        throw new NotImplementedException();
    }

    public AccountInfo ChangeGigabytesPlan(AccountInfo account, int newGigabytesAvailable)
    {
        throw new NotImplementedException();
    }

    public long BytesAvailable(AccountInfo account)
    {
        throw new NotImplementedException();
    }
}