using JabaBox.Core.Domain.Entities;

namespace JabaBox.Core.Domain.ServicesAbstractions;

public interface IAccountService
{
    AccountInfo RegisterAccount(string login, string password, int gigabyteAvailable);
    AccountInfo ChangePassword(string login, string oldPassword, string newPassword);
    AccountInfo ChangeGigabytesPlan(AccountInfo account, int newGigabytesAvailable);
    long BytesAvailable(AccountInfo account);
}